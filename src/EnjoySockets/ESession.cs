// Copyright (c) Luke Matt. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Net;
using System.Net.Sockets;

namespace EnjoySockets
{
    /// <summary>
    /// Represents a base class for a network session bound to a socket resource.
    /// <para/>
    /// Provides access to serialization and basic endpoint information.
    /// </summary>
    public abstract class ESession<T1> where T1 : ESocketResource
    {
        internal Guid UserId { get; set; }

        /// <summary>
        /// Gets the serializer instance used for encoding and decoding network messages in this session.
        /// </summary>
        public IESerializer ESerial { get; }

        /// <summary>
        /// Gets the remote endpoint of the connected socket.
        /// </summary>
        public EndPoint? EndPointSocket { get => SocketResource?.BasicSocket?.RemoteEndPoint; }

        /// <summary>
        /// Gets the address family (IPv4/IPv6) of the underlying socket connection.
        /// </summary>
        public AddressFamily? AddressFamilySocket { get => SocketResource?.BasicSocket?.AddressFamily; }

        internal T1? SocketResource;

        private protected readonly object _lock = new();

        /// <summary>
        /// Initializes a new instance of <see cref="ESession{T1}"/> bound to the specified socket resource.
        /// </summary>
        /// <param name="esr">The socket resource associated with this session.</param>
        public ESession(T1 esr)
        {
            SocketResource = esr;
            ESerial = esr.Config.ESerial;
        }

        internal virtual bool Start()
        {
            return SocketResource?.Run() ?? false;
        }

        /// <summary>
        /// Registers an instance in the resource and returns its generated ID.
        /// </summary>
        /// <param name="obj">
        /// The object to register. Must contain at least one non-static access method.
        /// </param>
        /// <returns>
        /// The generated instance ID (&gt; 1_000_000).
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="obj"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the instance cannot be registered due to an invalid type
        /// or missing access configuration.
        /// </exception>
        public long InstanceRegister(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            var id = SocketResource?.RegisterPrivateInstance(obj);

#if DEBUG
            if (id == null || id <= 0)
                throw new InvalidOperationException(
                    $"Private instance of type '{obj.GetType().FullName}' cannot be registered. No exist any one fit method for: {SocketResource?.SocketRole}");
#endif

            return id ?? 0;
        }

        /// <summary>
        /// Removes the specified instance from the resource.
        /// </summary>
        /// <param name="id">The ID of the instance to remove.</param>
        /// <returns>
        /// <see langword="true"/> if the instance was successfully removed; 
        /// <see langword="false"/> if the instance could not be removed or the ID is invalid.
        /// </returns>
        /// <remarks>
        /// After removal, any incoming data for this instance will be dropped.
        /// </remarks>
        public bool InstanceRemove(long id)
        {
            return SocketResource?.RemovePrivateInstance(id) ?? false;
        }

        /// <summary>
        /// Detaches all registered instances from the resource.
        /// </summary>
        /// <remarks>
        /// After detachment, any incoming data for these instances will be dropped.
        /// </remarks>
        public void InstanceDetach()
        {
            SocketResource?.ClearPrivateInstance();
        }

        /// <summary>
        /// Sends a message without a payload to the specified target.
        /// </summary>
        /// <inheritdoc cref="Send{T}(long, string, T)"/>
        public ValueTask<bool> Send(string target)
        {
            return Send(0, target);
        }

        /// <summary>
        /// Sends a message with a payload to the specified target.
        /// </summary>
        /// <inheritdoc cref="Send{T}(long, string, T)"/>
        public ValueTask<bool> Send<T>(string target, T? obj)
        {
            return Send(0, target, obj);
        }

        /// <summary>
        /// Sends a message without a payload to the specified target and specified instance.
        /// </summary>
        /// <inheritdoc cref="Send{T}(long, string, T)"/>
        public ValueTask<bool> Send(long instance, string target)
        {
            return Send<object>(instance, target, null);
        }

        /// <summary>
        /// Sends a message with a payload to the specified target and specified instance.
        /// </summary>
        /// <remarks>
        /// The message is considered sent once it is successfully handed off to the operating system.
        /// <para>
        /// This does not guarantee delivery to the receiving destination.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type of the payload being sent.</typeparam>
        /// <param name="instance">The destination to which instance the message is sent.</param>
        /// <param name="target">The destination to which the message is sent.</param>
        /// <param name="obj">The payload object to send. May be <see langword="null"/>.</param>
        /// <returns>
        /// <see langword="true"/> if the message was successfully passed to the operating system;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public virtual ValueTask<bool> Send<T>(long instance, string target, T? obj)
        {
            return ValueTask.FromResult(false);
        }

        /// <summary>
        /// Called when the connection has been successfully established.
        /// </summary>
        protected virtual void OnConnected() { }

        /// <summary>
        /// Called when the connection has been closed.
        /// </summary>
        /// <remarks>
        /// This method is invoked regardless of whether the disconnection
        /// was intentional or caused by an error.
        /// </remarks>
        protected virtual void OnDisconnected() { }

        internal const long MinUniqueId = 1_000_000;
        static long _lastId = Math.Max(MinUniqueId, DateTime.UtcNow.Ticks);
        /// <summary>
        /// Generates a thread-safe unique ID (long) for the entire application.
        /// </summary>
        public static long GetUniqueId()
        {
            return Interlocked.Increment(ref _lastId);
        }
    }
}
