// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Security.Cryptography;

namespace EnjoySockets
{
    /// <summary>
    /// Represents the base type for all configuration classes: <see cref="EConfigClient"/> and <see cref="EConfigServer"/>
    /// </summary>
    public abstract class EConfig
    {
        /// <summary>
        /// Message buffer size for send/receive in KB. 
        /// Minimum: 2 KB. Default: 300 KB.
        /// <para/>
        /// IMPORTANT: This value must be identical on both client and server.
        /// </summary>
        public ushort MessageBuffer { get; set; } = 300;

        /// <summary>
        /// Max packet size for socket.send in bytes. 
        /// Minimum: 1200 bytes. Default: 1300 bytes.
        /// <para/>
        /// IMPORTANT: This value must be identical on both client and server.
        /// </summary>
        public short MaxPacketSize { get; set; } = 1300;

        /// <summary>
        /// Time in milliseconds to wait for a packet acknowledgment (connect).
        /// If no acknowledgment is received within this time, the connection is dropped.
        /// <para/>
        /// Default: 2000 ms.
        /// Valid range: 100–8000 ms.
        /// </summary>
        public int ResponseTimeout { get; set; } = 2000;

        /// <summary>
        /// Gets or sets the heartbeat interval in seconds.
        /// Values less than 1 or greater than 3600 are reset to the default value.
        /// <para/>
        /// Server default: 30 seconds.
        /// Client default: 0 seconds (disabled).
        /// <para/>
        /// Client-side: a value of 0 disables the heartbeat.
        /// <para/>
        /// Server-side: cannot be disabled.
        /// </summary>
        public int Heartbeat { get; set; } = 30;

        /// <summary>
        /// ECDH curve used for key agreement. 
        /// Default: nistP384.
        /// </summary>
        public ECCurve Curve { get; set; } = ECCurve.NamedCurves.nistP384;

        /// <summary>
        /// Gets or sets the serializer implementation used for object serialization in the client/server communication layer.
        /// <para/>
        /// Default value: <see cref="ESerialMemoryPack"/> (IdSerializer = 0).
        /// </summary>
        public IESerializer ESerial { get; set; } = new ESerialMemoryPack();

        /// <summary>
        /// Creates a shallow copy of this configuration.
        /// </summary>
        public abstract EConfig Clone();
    }
}
