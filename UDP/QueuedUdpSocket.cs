using InjectorGames.SharedLibrary.Logs;
using System.Collections.Generic;

namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Queued UDP socket class (thread-safe)
    /// </summary>
    public class QueuedUdpSocket : UdpSocket, IQueuedUdpSocket
    {
        /// <summary>
        /// Maximum datagram count in the queue
        /// </summary>
        protected int maxDatagramCount;

        /// <summary>
        /// Datagram queue locker
        /// </summary>
        protected readonly object locker;
        /// <summary>
        /// Received datagram queue
        /// </summary>
        protected readonly Queue<Datagram> datagrams;

        /// <summary>
        /// Creates a new queued UDP socket abstract class instance
        /// </summary>
        public QueuedUdpSocket(ILogger logger, int maxDatagramCount) : base(logger)
        {
            this.maxDatagramCount = maxDatagramCount;

            locker = new object();
            datagrams = new Queue<Datagram>();
        }

        /// <summary>
        /// Dequeues next datagram from the queue (thread-safe)
        /// </summary>
        public Datagram DequeueNext()
        {
            lock (locker)
            {
                var datagram = datagrams.Dequeue();

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Dequeued UDP socket datagram. (remoteEndPoint: {datagram.ipEndPoint}, length: {datagram.Length}, type: {datagram.Type}))");

                return datagram;
            }
        }

        /// <summary>
        /// Dequeues all datagrams from the queue (thread-safe)
        /// </summary>
        public Datagram[] DequeueAll()
        {
            lock (locker)
            {
                var datagramArray = datagrams.ToArray();

                datagrams.Clear();

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Dequeued all UDP socket datagrams. (count: {datagramArray.Length}))");

                return datagramArray;
            }
        }

        /// <summary>
        /// On UDP socket datagram receive
        /// </summary>
        protected override void OnDatagramReceive(Datagram datagram)
        {
            lock (locker)
            {
                if (datagrams.Count < maxDatagramCount)
                {
                    datagrams.Enqueue(datagram);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Enqueued UDP socket datagram. (remoteEndPoint: {datagram.ipEndPoint}, length: {datagram.Length}, type: {datagram.Type}))");
                }
                else
                {
                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Failed to enqueued UDP socket datagram, queue is full. (remoteEndPoint: {datagram.ipEndPoint}, length: {datagram.Length}, type: {datagram.Type}))");
                }
            }
        }
    }
}
