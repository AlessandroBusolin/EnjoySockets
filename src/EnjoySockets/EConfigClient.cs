// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace EnjoySockets
{
    /// <summary>
    /// Represents client-side configuration.
    /// </summary>
    public sealed class EConfigClient : EConfig
    {
        /// <summary>
        /// The maximum time, in seconds, to wait for a server response when calling <see cref="EClient.Connect(EAddress?)"/>.
        /// <para/>
        /// Defaults is 3 seconds. Valid range is 1–30 seconds.
        /// </summary>
        public int ConnectTimeout { get; set; } = 3;

        /// <summary>
        /// Configuration for <see cref="EClient"/> and all classes derived from it.
        /// </summary>
        public EConfigClient()
        {
            Heartbeat = 0;
        }

        /// <inheritdoc cref="EConfig.Clone"/>
        public override EConfigClient Clone()
        {
            return new EConfigClient
            {
                MessageBuffer = MessageBuffer < 2 ? (ushort)300 : MessageBuffer,
                MaxPacketSize = MaxPacketSize < 1200 ? (short)1300 : MaxPacketSize,
                ResponseTimeout = ResponseTimeout < 100 || ResponseTimeout > 8000 ? 2000 : ResponseTimeout,
                ConnectTimeout = ConnectTimeout < 1 || ConnectTimeout > 30 ? 3 : ConnectTimeout,
                Heartbeat = Heartbeat < 0 || Heartbeat > 3600 ? 0 : Heartbeat,
                Curve = Curve,
                ESerial = ESerial
            };
        }
    }
}
