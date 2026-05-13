// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace EnjoySockets
{
    public enum ETCPSocketType
    {
        Server, Client
    }

    internal sealed class ETCPSocket
    {
        //length packet from socket
        public const short PacketPrefixLength = 2;
        //header aes-gcm (nonce, tag)
        public const short PacketEncryptHeader = 28;
        //packet header in plain msg
        public const short PacketHeader = 29;

        public const int MaxPacketSizeConnect = 1330;

        public const int MaxCachedResponses = 50;
        public const int MinBufferSlotSizeBytes = 30;

        #region Receive methods

        internal static async ValueTask<ReadOnlyMemory<byte>> Receive(Socket socket, EReceiveArgs args)
        {
            args.StartPrepare(PacketPrefixLength);
            if (await ReadAsync(socket, args))
            {
                var dataLength = BinaryPrimitives.ReadUInt16LittleEndian(args.GetSaveBytesSpan());
                if (dataLength <= 0 || dataLength > MaxPacketSizeConnect)
                    return Memory<byte>.Empty;

                args.StartPrepare(dataLength);
                if (await ReadAsync(socket, args))
                    return args.GetSaveBytes();
            }
            return Memory<byte>.Empty;
        }

        internal static async ValueTask<ReadOnlyMemory<byte>> Receive(Socket socket, EAesGcm aes, EReceiveArgs args)
        {
            args.StartPrepare(2);
            if (await ReadAsync(socket, args))
            {
                var dataLength = BinaryPrimitives.ReadUInt16LittleEndian(args.GetSaveBytesSpan());
                if (dataLength <= PacketEncryptHeader || dataLength > MaxPacketSizeConnect)
                    return Memory<byte>.Empty;

                args.StartPrepare(dataLength);
                if (await ReadAsync(socket, args))
                    return args.GetSaveBytes(aes);
            }
            return Memory<byte>.Empty;
        }

        internal static async ValueTask<ReadOnlyMemory<byte>> ReceiveWithTimeout(ValueTask<ReadOnlyMemory<byte>> receiveTask, int timeoutMilliseconds)
        {
            var rT = receiveTask.AsTask();
            if (await Task.WhenAny(rT, Task.Delay(timeoutMilliseconds)) != rT)
                return Memory<byte>.Empty;
            else
                return rT.Result;
        }

        internal static int TryRead(Socket socket, EReceiveArgs args, int toRead)
        {
            args.StartPrepare(toRead);
            int totalRead = 0;
            while (totalRead < args.MaxLengthArray)
            {
                args.PrepareBuffer();
                bool pending;
                try
                {
                    pending = socket.ReceiveAsync(args.SAEA);
                }
                catch
                {
                    return -1;
                }

                if (!pending)
                {
                    int bytesRead = args.SAEA.BytesTransferred;

                    if (bytesRead <= 0)
                        return -1;

                    totalRead += bytesRead;

                    if (totalRead == args.MaxLengthArray)
                        return totalRead;

                    args.AddOffset(bytesRead);
                }
                else
                    return totalRead;
            }
            return totalRead;
        }

        internal static async ValueTask<bool> ReadContinueAsync(Socket socket, EReceiveArgs args, int read)
        {
            int totalRead = read;

            int bytesRead = await args.WaitForCompletionAsync();

            if (bytesRead <= 0)
                return false;

            totalRead += bytesRead;

            if (totalRead == args.MaxLengthArray)
                return true;

            args.AddOffset(bytesRead);

            while (totalRead < args.MaxLengthArray)
            {
                args.PrepareBuffer();
                bool pending;
                try
                {
                    pending = socket.ReceiveAsync(args.SAEA);
                }
                catch
                {
                    return false;
                }

                bytesRead = pending
                    ? await args.WaitForCompletionAsync()
                    : args.SAEA.BytesTransferred;

                if (bytesRead <= 0)
                    return false;

                totalRead += bytesRead;

                if (totalRead == args.MaxLengthArray)
                    return true;

                args.AddOffset(bytesRead);
            }
            return true;
        }

        internal static async ValueTask<bool> ReadAsync(Socket socket, EReceiveArgs args)
        {
            int totalRead = 0;
            while (totalRead < args.MaxLengthArray)
            {
                args.PrepareBuffer();
                bool pending;
                try
                {
                    pending = socket.ReceiveAsync(args.SAEA);
                }
                catch
                {
                    return false;
                }

                int bytesRead = pending
                    ? await args.WaitForCompletionAsync()
                    : args.SAEA.BytesTransferred;

                if (bytesRead <= 0)
                    return false;

                totalRead += bytesRead;

                if (totalRead == args.MaxLengthArray)
                    return true;

                args.AddOffset(bytesRead);
            }
            return true;
        }

        internal static async ValueTask<bool> Read(Socket socket, Memory<byte> buffer)
        {
            int bytesRead = 0;
            while (bytesRead < buffer.Length)
            {
                try
                {
#if NET8_0
                    int read = await socket.ReceiveAsync(buffer.Slice(bytesRead, buffer.Length - bytesRead));
#else
                    var slice = buffer.Slice(bytesRead, buffer.Length - bytesRead);
                    var temp = slice.ToArray();
                    int read = await socket.ReceiveAsync(new ArraySegment<byte>(temp), SocketFlags.None);
                    temp.AsMemory(0, read).CopyTo(slice);
#endif
                    if (read < 1)
                        return false;

                    bytesRead += read;
                }
                catch
                {
                    return false;
                }
            }
            return buffer.Length == bytesRead;
        }

        #endregion

        #region Send methods

        internal static ValueTask<bool> Send(Socket socket, ReadOnlyMemory<byte> data)
        {
            if (data.Length > MaxPacketSizeConnect)
                return ValueTask.FromResult(false);

            return Write(socket, data);
        }

        internal static ValueTask<bool> Send(Socket? socket, ESendArgs args, ReadOnlyMemory<byte> data)
        {
            if (socket == null || !args.SetToSend(data))
                return ValueTask.FromResult(false);

            return Write(socket, args);
        }

        internal static ValueTask<bool> Send(Socket? socket, ESendArgs args, EAesGcm aes, ReadOnlyMemory<byte> data)
        {
            if (socket == null || !args.SetToSend(data.Span, aes))
                return ValueTask.FromResult(false);

            return Write(socket, args);
        }

        internal static ValueTask<bool> Write(Socket? socket, ESendArgs args)
        {
            if (socket == null)
                return ValueTask.FromResult(false);

            int sentSoFar = 0;
            while (sentSoFar < args.MaxLengthArray)
            {
                args.Prepare();

                bool willRaiseEvent;
                try
                {
                    willRaiseEvent = socket.SendAsync(args.SAEA);
                }
                catch
                {
                    return ValueTask.FromResult(false);
                }

                if (!willRaiseEvent)
                {
                    int bytesSent = args.SAEA.BytesTransferred;
                    if (bytesSent <= 0)
                        return ValueTask.FromResult(false);

                    sentSoFar += bytesSent;

                    if (sentSoFar == args.MaxLengthArray)
                        return ValueTask.FromResult(true);

                    args.AddOffset(bytesSent);
                    continue;
                }
                return WriteAsync(socket, args, sentSoFar);
            }
            return ValueTask.FromResult(sentSoFar == args.MaxLengthArray);
        }

        static async ValueTask<bool> WriteAsync(Socket socket, ESendArgs args, int sentSoFar)
        {
            int bytesSent = await args.WaitForCompletionAsync();
            if (bytesSent <= 0)
                return false;

            sentSoFar += bytesSent;

            if (sentSoFar == args.MaxLengthArray)
                return true;

            args.AddOffset(bytesSent);

            while (sentSoFar < args.MaxLengthArray)
            {
                args.Prepare();

                bool willRaiseEvent;
                try
                {
                    willRaiseEvent = socket.SendAsync(args.SAEA);
                }
                catch
                {
                    return false;
                }

                if (!willRaiseEvent)
                    bytesSent = args.SAEA.BytesTransferred;
                else
                    bytesSent = await args.WaitForCompletionAsync();

                if (bytesSent <= 0)
                    return false;

                sentSoFar += bytesSent;

                if (sentSoFar == args.MaxLengthArray)
                    return true;

                args.AddOffset(bytesSent);
            }
            return sentSoFar == args.MaxLengthArray;
        }

        static async ValueTask<bool> Write(Socket socket, ReadOnlyMemory<byte> data)
        {
            var totalBytesToSend = data.Length;
            var offset = 0;
            while (offset < totalBytesToSend)
            {
                try
                {
#if NET8_0
                    var slice = data.Slice(offset);
                    var written = await socket.SendAsync(slice);
#else
                    int written;
                    var slice = data.Slice(offset);

                    if (MemoryMarshal.TryGetArray(slice, out ArraySegment<byte> segment))
                    {
                        written = await socket.SendAsync(segment, SocketFlags.None);
                    }
                    else
                    {
                        byte[] temp = slice.ToArray();
                        written = await socket.SendAsync(new ArraySegment<byte>(temp), SocketFlags.None);
                    }
#endif
                    if (written <= 0)
                        return false;

                    offset += written;

                    if (offset == totalBytesToSend)
                        return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        #endregion

        internal static void ShutdownAndClose(Socket? socket)
        {
            try
            {
                socket?.Shutdown(SocketShutdown.Send);
            }
            catch { }
            finally
            {
                Close(socket);
            }
        }

        internal static void Close(Socket? socket)
        {
            try
            {
                socket?.Dispose();
            }
            catch { }
        }
    }
}
