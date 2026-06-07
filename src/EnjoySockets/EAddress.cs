using System.Net;

namespace EnjoySockets
{
    /// <summary>
    /// Represents a network endpoint address consisting of an IP address and port.
    /// </summary>
    public class EAddress
    {
        /// <summary>
        /// Gets or sets the default local IP address used when creating endpoints.
        /// <para/>
        /// Default value is <see cref="IPAddress.Loopback"/>.
        /// </summary>
        public static IPAddress LocalIP { get; set; } = IPAddress.Loopback;

        /// <summary>
        /// Gets the underlying IP endpoint represented by this address.
        /// </summary>
        public IPEndPoint? EndPoint { get; }

        private EAddress() { }

        private EAddress(IPEndPoint? endPoint)
        {
            EndPoint = endPoint;
        }

        /// <summary>
        /// Creates an <see cref="EAddress"/> using the <c><see cref="EAddress.LocalIP"/></c> and port <c>3001</c>.
        /// </summary>
        public static EAddress Get()
        {
            return new EAddress(new IPEndPoint(LocalIP, 3001));
        }

        /// <summary>
        /// Creates an <see cref="EAddress"/> using the <c><see cref="EAddress.LocalIP"/></c> and the specified port.
        /// </summary>
        /// <param name="port">The port number.</param>
        public static EAddress Get(int port)
        {
            return Get(LocalIP, port);
        }

        /// <summary>
        /// Creates an <see cref="EAddress"/> from the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address as a string.</param>
        /// <param name="port">The port number.</param>
        /// <returns>
        /// An <see cref="EAddress"/> instance representing the endpoint.
        /// <para/>
        /// If the input is invalid, an empty instance is returned.
        /// </returns>
        public static EAddress Get(string ipAddress, int port)
        {
            try
            {
                return new EAddress(new IPEndPoint(IPAddress.Parse(ipAddress), port));
            }
            catch
            {
                return new EAddress();
            }
        }

        /// <summary>
        /// Creates an <see cref="EAddress"/> from the specified IP address and port.
        /// </summary>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="port">The port number.</param>
        /// <returns>
        /// An <see cref="EAddress"/> instance representing the endpoint.
        /// <para/>
        /// If the input is invalid, an empty instance is returned.
        /// </returns>
        public static EAddress Get(IPAddress ipAddress, int port)
        {
            try
            {
                return new EAddress(new IPEndPoint(ipAddress, port));
            }
            catch
            {
                return new EAddress();
            }
        }

        /// <summary>
        /// Creates an <see cref="EAddress"/> from an existing <see cref="IPEndPoint"/>.
        /// </summary>
        /// <param name="endPoint">The endpoint to wrap.</param>
        /// <returns>An <see cref="EAddress"/> instance or an empty instance if <paramref name="endPoint"/> is null.</returns>
        public static EAddress Get(IPEndPoint? endPoint)
        {
            return new EAddress(endPoint);
        }
    }
}
