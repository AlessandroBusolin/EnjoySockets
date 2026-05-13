using MemoryPack;
using System.Buffers;

namespace EnjoySockets
{
    public sealed class ESerialMemoryPack : IESerializer
    {
        public int IdSerializer => 0;

        public T? Deserialize<T>(ReadOnlySpan<byte> data)
        {
            try
            {
                return MemoryPackSerializer.Deserialize<T>(data);
            }
            catch { return default; }
        }

        public object? Deserialize(ReadOnlySpan<byte> data, Type type)
        {
            try
            {
                return MemoryPackSerializer.Deserialize(type, data);
            }
            catch { return default; }
        }

        public bool Deserialize(Type type, ReadOnlySpan<byte> data, ref object? obj)
        {
            try
            {
                MemoryPackSerializer.Deserialize(type, data, ref obj);
                return true;
            }
            catch { return false; }
        }

        public object? Deserialize(ReadOnlySequence<byte> data, Type type)
        {
            try
            {
                return MemoryPackSerializer.Deserialize(type, data);
            }
            catch { return default; }
        }

        public bool Deserialize(ReadOnlySequence<byte> data, Type type, ref object? obj)
        {
            try
            {
                MemoryPackSerializer.Deserialize(type, data, ref obj);
                return true;
            }
            catch { return false; }
        }

        public byte[]? Serialize<T>(T? myObj)
        {
            try
            {
                return MemoryPackSerializer.Serialize(myObj);
            }
            catch { return null; }
        }

        public int Serialize<T>(EArrayBufferWriter buffer, T? myObj)
        {
            try
            {
                buffer.ResetWrittenCount();
                MemoryPackSerializer.Serialize(buffer, myObj);
                return buffer.WrittenSpan.Length;
            }
            catch { return 0; }
        }

        public int Serialize(EArrayBufferWriter buffer, object? myObj, Type? t)
        {
            try
            {
                if (t == null) return 0;
                buffer.ResetWrittenCount();
                MemoryPackSerializer.Serialize(t, buffer, myObj);
                return buffer.WrittenSpan.Length;
            }
            catch { return 0; }
        }
    }
}
