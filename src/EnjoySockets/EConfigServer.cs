// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace EnjoySockets
{
    /// <summary>
    /// Represents server-side configuration.
    /// </summary>
    public sealed class EConfigServer : EConfig
    {
        /// <summary>
        /// Indicates the number of sockets in queue to accept (Socket.Listen).
        /// <para/>
        /// Default is 128. Valid range is 1–4096.
        /// </summary>
        public int QueueSocketToAccept { get; set; } = 128;

        /// <summary>
        /// Indicates the max number of sockets connect to server.
        /// <para/>
        /// Default is 5000.
        /// </summary>
        public int MaxSockets { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the keep-alive duration in seconds.
        /// <para/>
        /// Valid range is 10–43200 seconds (12 hours).
        /// Values outside this range are automatically reset to the default value.
        /// <para/>
        /// Default value: 60 seconds.
        /// </summary>
        public int KeepAlive { get; set; } = 60;

        /// <summary>
        /// Provides configuration settings used by <see cref="EServer"/> and propagated to created <see cref="EServerSession"/> instances.
        /// </summary>
        public EConfigServer() { }

        /// <inheritdoc cref="EConfig.Clone"/>
        public override EConfigServer Clone()
        {
            return new EConfigServer
            {
                QueueSocketToAccept = QueueSocketToAccept < 1 || QueueSocketToAccept > 4096 ? 128 : QueueSocketToAccept,
                MaxSockets = MessageBuffer < 1 ? 5000 : MaxSockets,
                MessageBuffer = MessageBuffer < 2 ? (ushort)300 : MessageBuffer,
                MaxPacketSize = MaxPacketSize < 1200 ? (short)1300 : MaxPacketSize,
                KeepAlive = KeepAlive < 10 || KeepAlive > 43200 ? 60 : KeepAlive,
                ResponseTimeout = ResponseTimeout < 100 || ResponseTimeout > 8000 ? 2000 : ResponseTimeout,
                Heartbeat = Heartbeat < 1 || Heartbeat > 3600 ? 30 : Heartbeat,
                Curve = Curve,
                ESerial = ESerial
            };
        }
    }
}
