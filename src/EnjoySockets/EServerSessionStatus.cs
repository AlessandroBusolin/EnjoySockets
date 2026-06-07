// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace EnjoySockets
{
    /// <summary>
    /// Represents the lifecycle state of a single server-side session instance.
    /// </summary>
    public enum EServerSessionStatus
    {
        /// <summary>
        /// The session is active and operational.
        /// </summary>
        Alive,

        /// <summary>
        /// The session is attempting to reconnect.
        /// </summary>
        ReconnectAttempt,

        /// <summary>
        /// The session is temporarily bypassed while reconnection logic is executed.
        /// </summary>
        BypassToReconnect,

        /// <summary>
        /// The session is no longer usable and should be detached from all references and collections, allowing it to be garbage collected.
        /// </summary>
        Dead
    }
}
