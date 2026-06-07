using System.Buffers;

namespace EnjoySockets
{
    /// <summary>
    /// Defines a contract for binary serialization and deserialization used by the networking layer.
    /// </summary>
    public interface IESerializer
    {
        /// <summary>
        /// Gets the serializer identifier used for runtime selection.
        /// </summary>
        int IdSerializer { get; }

        /// <summary>
        /// Deserializes an object from a contiguous memory buffer.
        /// </summary>
        T? Deserialize<T>(ReadOnlySpan<byte> data);

        /// <summary>
        /// Deserializes an object from a sequence of memory buffers.
        /// </summary>
        T? Deserialize<T>(ReadOnlySequence<byte> data);

        /// <summary>
        /// Deserializes an object of the specified runtime type.
        /// </summary>
        object? Deserialize(ReadOnlySpan<byte> data, Type type);

        /// <summary>
        /// Deserializes an object into an existing reference instance.
        /// </summary>
        bool Deserialize(Type type, ReadOnlySpan<byte> data, ref object? obj);

        /// <summary>
        /// Deserializes an object of the specified runtime type from a memory sequence.
        /// </summary>
        object? Deserialize(ReadOnlySequence<byte> data, Type type);

        /// <summary>
        /// Deserializes an object into an existing reference instance using a memory sequence.
        /// </summary>
        bool Deserialize(ReadOnlySequence<byte> data, Type type, ref object? obj);

        /// <summary>
        /// Serializes an object into a new byte array.
        /// </summary>
        byte[]? Serialize<T>(T? myObj);

        /// <summary>
        /// Serializes an object into a buffer writer and returns written byte count.
        /// </summary>
        int Serialize<T>(EArrayBufferWriter buffer, T? myObj);

        /// <summary>
        /// Serializes an object of runtime type into a buffer writer.
        /// </summary>
        int Serialize(EArrayBufferWriter buffer, object? myObj, Type? t);
    }
}