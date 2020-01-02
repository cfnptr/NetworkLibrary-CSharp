using InjectorGames.SharedLibrary.Logs;
using System.Net;

namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// UDP socket interface
    /// </summary>
    public interface IUdpSocket
    {
        /// <summary>
        /// Is UDP socket threads still running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// UDP socket logger
        /// </summary>
        ILogger Logger { get; }
        /// <summary>
        /// UDP socket local ip end point
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        void Start(IPEndPoint localEndPoint);
        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        void Start(IPAddress address, int port);
        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        void Start();

        /// <summary>
        /// Closes UDP socket socket and stops receive thread
        /// </summary>
        void Close();

        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        int Send(byte[] buffer, int offset, int count, IPEndPoint remoteEndPoint);
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        int Send(byte[] data, IPEndPoint remoteEndPoint);
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        int Send(Datagram datagram);
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        int Send(IUdpRequestResponse requestResponse, IPEndPoint remoteEndPoint);
    }
}
