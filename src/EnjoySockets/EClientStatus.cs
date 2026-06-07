// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace EnjoySockets
{
    /// <summary>
    /// Represents the lifecycle state of a reusable client connection.
    /// </summary>
    public enum EClientStatus
    {
        /// <summary>
        /// The client is actively connected to the server.
        /// </summary>
        Connected,

        /// <summary>
        /// The client is attempting to reconnect to the server.
        /// </summary>
        ReconnectAttempt,

        /// <summary>
        /// The client is not currently connected to the server.
        /// </summary>
        Disconnected
    }
}
