namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Queued UDP socket interface (thread-safe)
    /// </summary>
    public interface IQueuedUdpSocket : IUdpSocket
    {
        /// <summary>
        /// Dequeues next datagram from the queue (thread-safe)
        /// </summary>
        Datagram DequeueNext();

        /// <summary>
        /// Dequeues all datagrams from the queue (thread-safe)
        /// </summary>
        Datagram[] DequeueAll();
    }
}
