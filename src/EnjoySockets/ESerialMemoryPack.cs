// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MemoryPack;
using System.Buffers;
using System.Diagnostics;

namespace EnjoySockets
{
    /// <summary>
    /// Default serializer for the EnjoySockets framework using 'MemoryPack'. Handles binary serialization for network messages.
    /// </summary>
    public class ESerialMemoryPack : IESerializer
    {
        /// <summary>
        /// Gets the serializer identifier used for runtime selection.
        /// </summary>
        public int IdSerializer => 0;

        /// <summary>
        /// Deserializes a strongly-typed object from a contiguous memory buffer.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="data">The input binary data.</param>
        /// <returns>The deserialized object, or <c>default</c> if deserialization fails.</returns>
        public T? Deserialize<T>(ReadOnlySpan<byte> data)
        {
            try
            {
                return MemoryPackSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "<T>(ReadOnlySpan<byte>)", ex);
                return default;
            }
        }

        /// <summary>
        /// Deserializes a strongly-typed object from a sequence of memory buffers.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize.</typeparam>
        /// <param name="data">The input binary data sequence.</param>
        /// <returns>The deserialized object, or <c>default</c> if deserialization fails.</returns>
        public T? Deserialize<T>(ReadOnlySequence<byte> data)
        {
            try
            {
                return MemoryPackSerializer.Deserialize<T>(data);
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "<T>(ReadOnlySequence<byte>)", ex);
                return default;
            }
        }

        /// <summary>
        /// Deserializes an object of the specified runtime type from a contiguous memory buffer.
        /// </summary>
        /// <param name="data">The input binary data.</param>
        /// <param name="type">The target object type.</param>
        /// <returns>The deserialized object, or <c>null</c> if deserialization fails.</returns>
        public object? Deserialize(ReadOnlySpan<byte> data, Type type)
        {
            try
            {
                return MemoryPackSerializer.Deserialize(type, data);
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "(ReadOnlySpan<byte>, Type)", ex);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object of the specified runtime type into an existing reference.
        /// </summary>
        /// <param name="type">The target object type.</param>
        /// <param name="data">The input binary data.</param>
        /// <param name="obj">The object instance to populate.</param>
        /// <returns><c>true</c> if deserialization succeeded; otherwise <c>false</c>.</returns>
        public bool Deserialize(Type type, ReadOnlySpan<byte> data, ref object? obj)
        {
            try
            {
                MemoryPackSerializer.Deserialize(type, data, ref obj);
                return true;
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "(Type, ReadOnlySpan<byte>, ref object)", ex);
                return false;
            }
        }

        /// <summary>
        /// Deserializes an object of the specified runtime type from a sequence of memory buffers.
        /// </summary>
        public object? Deserialize(ReadOnlySequence<byte> data, Type type)
        {
            try
            {
                return MemoryPackSerializer.Deserialize(type, data);
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "(ReadOnlySequence<byte>, Type)", ex);
                return null;
            }
        }

        /// <summary>
        /// Deserializes an object of the specified runtime type into an existing reference using a memory sequence.
        /// </summary>
        public bool Deserialize(ReadOnlySequence<byte> data, Type type, ref object? obj)
        {
            try
            {
                MemoryPackSerializer.Deserialize(type, data, ref obj);
                return true;
            }
            catch (Exception ex)
            {
                Log(nameof(Deserialize) + "(ReadOnlySequence<byte>, Type, ref object)", ex);
                return false;
            }
        }

        /// <summary>
        /// Serializes an object into a new byte array.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="myObj">The object to serialize.</param>
        /// <returns>The serialized byte array, or <c>null</c> if serialization fails.</returns>
        public byte[]? Serialize<T>(T? myObj)
        {
            try
            {
                return MemoryPackSerializer.Serialize(myObj);
            }
            catch (Exception ex)
            {
                Log(nameof(Serialize) + "<T>(T)", ex);
                return null;
            }
        }

        /// <summary>
        /// Serializes an object into a preallocated buffer writer.
        /// </summary>
        /// <typeparam name="T">The type of object to serialize.</typeparam>
        /// <param name="buffer">The target buffer writer.</param>
        /// <param name="myObj">The object to serialize.</param>
        /// <returns>The number of written bytes.</returns>
        public int Serialize<T>(EArrayBufferWriter buffer, T? myObj)
        {
            try
            {
                buffer.ResetWrittenCount();
                MemoryPackSerializer.Serialize(buffer, myObj);
                return buffer.WrittenSpan.Length;
            }
            catch (Exception ex)
            {
                Log(nameof(Serialize) + "<T>(EArrayBufferWriter, T)", ex);
                return 0;
            }
        }

        /// <summary>
        /// Serializes an object of a runtime type into a preallocated buffer writer.
        /// </summary>
        /// <param name="buffer">The target buffer writer.</param>
        /// <param name="myObj">The object to serialize.</param>
        /// <param name="t">The runtime type of the object.</param>
        /// <returns>The number of written bytes.</returns>
        public int Serialize(EArrayBufferWriter buffer, object? myObj, Type? t)
        {
            try
            {
                if (t == null) return 0;
                buffer.ResetWrittenCount();
                MemoryPackSerializer.Serialize(t, buffer, myObj);
                return buffer.WrittenSpan.Length;
            }
            catch (Exception ex)
            {
                Log(nameof(Serialize) + "(EArrayBufferWriter, object, Type)", ex);
                return 0;
            }
        }

        /// <summary>
        /// Logs serialization or deserialization errors (debug only).
        /// </summary>
        [Conditional("DEBUG")]
        public static void Log(string method, Exception ex)
        {
            Debug.WriteLine($"{method} - {ex}");
        }
    }
}
