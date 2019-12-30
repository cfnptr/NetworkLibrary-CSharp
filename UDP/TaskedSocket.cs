
// Copyright 2019 Nikita Fediuchin (QuantumBranch)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using OpenSharedLibrary.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenNetworkLibrary.UDP
{
    /// <summary>
    /// Tasked UDP socket abstract class
    /// </summary>
    public abstract class TaskedSocket : UdpSocket, ITaskedSocket
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
        public TaskedSocket(int maxTaskCount, ILogger logger) : base(logger)
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
