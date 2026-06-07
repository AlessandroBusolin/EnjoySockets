// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
namespace EnjoySockets
{
    /// <summary>
    /// Specifies how communication channels are managed for endpoints (methods).
    /// </summary>
    public enum EChannelType
    {
        /// <summary>
        /// A dedicated channel is created per socket.
        /// </summary>
        Private,
        /// <summary>
        /// A single global channel is shared across all sockets.
        /// </summary>
        Share
    }

    /// <summary>
    /// Specifies channel configuration for a field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EAttrChannel : Attribute
    {
        private EChannelType channelType;
        /// <summary>
        /// Defines how the underlying message channel is resolved for the target method.
        /// Messages are always routed to this method.
        /// <para/>
        /// Default value is <see cref="EChannelType.Private"/>.
        /// </summary>
        public EChannelType ChannelType { get { return channelType; } set { channelType = value; } }

        private ushort channelTasks = 1;
        /// <summary>
        /// Specifies the number of worker tasks responsible for processing and executing messages for this channel.
        /// <para/>
        /// Default value is 1.
        /// </summary>
        public ushort ChannelTasks { get { return channelTasks; } set { channelTasks = value; } }
    }
}
