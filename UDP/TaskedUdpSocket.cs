using InjectorGames.SharedLibrary.Logs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Tasked UDP socket abstract class
    /// </summary>
    public abstract class TaskedUdpSocket : UdpSocket, ITaskedUdpSocket
    {
        /// <summary>
        /// Still active task count
        /// </summary>
        protected int taskCount;
        /// <summary>
        /// Maximum active task count
        /// </summary>
        protected int maxTaskCount;

        /// <summary>
        /// Still active task count
        /// </summary>
        public int TaskCount => taskCount;

        /// <summary>
        /// Creates a new tasked UDP socket abstract class instance
        /// </summary>
        public TaskedUdpSocket(int maxTaskCount, ILogger logger) : base(logger)
        {
            taskCount = 0;
            this.maxTaskCount = maxTaskCount;
        }

        /// <summary>
        /// On UDP socket datagram receive
        /// </summary>
        protected override void OnDatagramReceive(Datagram datagram)
        {
            if (taskCount < maxTaskCount)
            {
                Task.Factory.StartNew(OnDatagramReceiveLogic, datagram);
            }
            else
            {
                if (logger.Log(LogType.Trace))
                    logger.Trace("Failed to start tasked UDP socket task, maximum active task count.");
            }
        }

        /// <summary>
        /// On UDP socket datagram receive task logic
        /// </summary>
        protected void OnDatagramReceiveLogic(object state)
        {
            if (logger.Log(LogType.Trace))
                logger.Trace("Started tasked UDP socket task.");

            Interlocked.Increment(ref taskCount);

            try
            {
                OnTaskedDatagramReceive((Datagram)state);
            }
            catch (Exception exception)
            {
                OnReceiveTaskException(exception);
            }

            Interlocked.Decrement(ref taskCount);

            if (logger.Log(LogType.Trace))
                logger.Trace("Finished tasked UDP socket task.");
        }

        /// <summary>
        /// On UDP socket receive task exception
        /// </summary>
        protected virtual void OnReceiveTaskException(Exception exception)
        {
            if (logger.Log(LogType.Fatal))
                logger.Fatal($"Tasked UDP socket request task exception. {exception}");

            Close();
        }

        /// <summary>
        /// On UDP socket tasked datagram receive
        /// </summary>
        protected abstract void OnTaskedDatagramReceive(Datagram datagram);
    }
}
