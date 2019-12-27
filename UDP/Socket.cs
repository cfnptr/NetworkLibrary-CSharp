
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OpenNetworkLibrary.UDP
{
    /// <summary>
    /// UDP socket abstract class
    /// </summary>
    public abstract class Socket : ISocket
    {
        /// <summary>
        /// Maximal UDP datagram/packet size
        /// </summary>
        public const int MaxUdpSize = 65536;
        /// <summary>
        /// Default request time out value in the milliseconds
        /// </summary>
        public const int RequestTimeOut = 5000;

        /// <summary>
        /// UDP server logger
        /// </summary>
        protected readonly ILogger logger;
        /// <summary>
        /// UDP receive socket
        /// </summary>
        protected readonly System.Net.Sockets.Socket socket;
        /// <summary>
        /// UDP receive thread
        /// </summary>
        protected readonly Thread receiveThread;

        /// <summary>
        /// Is UDP socket threads still running
        /// </summary>
        protected bool isRunning;

        /// <summary>
        /// Is UDP socket threads still running
        /// </summary>
        public bool IsRunning => isRunning;

        /// <summary>
        /// UDP socket logger
        /// </summary>
        public ILogger Logger => logger;
        /// <summary>
        /// UDP socket local ip end point
        /// </summary>
        public IPEndPoint LocalEndPoint => (IPEndPoint)socket.LocalEndPoint;

        /// <summary>
        /// Creates a new UDP socket abstract class instance
        /// </summary>
        public Socket(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException();
            socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receiveThread = new Thread(ReceiveThreadLogic) { IsBackground = true, };
        }

        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        public void Start(IPEndPoint localEndPoint)
        {
            if (isRunning)
                return;

            isRunning = true;

            socket.Bind(localEndPoint);
            receiveThread.Start();

            if (logger.Log(LogType.Info))
                logger.Info("UDP socket started.");
        }
        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        public void Start(IPAddress address, int port)
        {
            var localEndPoint = new IPEndPoint(address, port);
            Start(localEndPoint);
        }
        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        public void Start()
        {
            var localEndPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
            Start(localEndPoint);
        }
        /// <summary>
        /// Closes UDP socket socket and stops receive thread
        /// </summary>
        public void Close()
        {
            if (!isRunning)
                return;

            isRunning = false;

            try
            {
                socket.Close();

                if (logger.Log(LogType.Debug))
                    logger.Debug("UDP socket instance closed.");
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            try
            {
                if (logger.Log(LogType.Debug))
                    logger.Debug("Waiting for UDP socket receive thread...");

                if (receiveThread != Thread.CurrentThread)
                    receiveThread.Join();
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            if (logger.Log(LogType.Info))
                logger.Info("UDP socket closed.");
        }

        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(byte[] buffer, int offset, int count, IPEndPoint remoteEndPoint)
        {
            var result = socket.SendTo(buffer, offset, count, SocketFlags.None, remoteEndPoint);

            if (logger.Log(LogType.Trace))
                logger.Trace($"Sended UDP socket datagram. (remoteEndPoint: {remoteEndPoint}, sendedBytes: {result})");

            return result;
        }
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(byte[] data, IPEndPoint remoteEndPoint)
        {
            var result = socket.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndPoint);

            if (logger.Log(LogType.Trace))
                logger.Trace($"Sended UDP socket datagram. (remoteEndPoint: {remoteEndPoint}, sendedBytes: {result})");

            return result;
        }
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(Datagram datagram)
        {
            var result = socket.SendTo(datagram.data, 0, datagram.data.Length, SocketFlags.None, datagram.ipEndPoint);

            if (logger.Log(LogType.Trace))
                logger.Trace($"Sended UDP socket datagram. (remoteEndPoint: {datagram.ipEndPoint}, sendedBytes: {result})");

            return result;
        }
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(IRequestResponse requestResponse, IPEndPoint remoteEndPoint)
        {
            var data = requestResponse.ToData();
            var result = socket.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndPoint);

            if (logger.Log(LogType.Trace))
                logger.Trace($"Sended UDP socket datagram. (remoteEndPoint: {remoteEndPoint}, sendedBytes: {result})");

            return result;
        }

        /// <summary>
        /// Receive thread delegate method
        /// </summary>
        protected void ReceiveThreadLogic()
        {
            if (logger.Log(LogType.Debug))
                logger.Debug("UDP socket receive thread started.");

            var buffer = new byte[MaxUdpSize];

            while (isRunning)
            {
                try
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
                    var count = socket.ReceiveFrom(buffer, 0, MaxUdpSize, SocketFlags.None, ref endPoint);

                    if (count < Datagram.HeaderByteSize)
                        continue;

                    var data = new byte[count];
                    Buffer.BlockCopy(buffer, 0, data, 0, count);

                    var datagram = new Datagram(data, (IPEndPoint)endPoint);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Received UDP socket datagram. (remoteEndPoint: {datagram.ipEndPoint}, length: {datagram.Length}, type: {datagram.Type})");

                    OnDatagramReceive(datagram);
                }
                catch (Exception exception)
                {
                    OnReceiveThreadException(exception);
                }
            }

            if (logger.Log(LogType.Debug))
                logger.Debug("UDP socket receive thread stopped.");
        }

        /// <summary>
        /// On UDP socket close exception
        /// </summary>
        protected virtual void OnCloseException(Exception exception)
        {
            if (logger.Log(LogType.Fatal))
                logger.Fatal($"Failed to close UDP socket. {exception}");
        }
        /// <summary>
        /// On UDP socket receive thread exception
        /// </summary>
        protected virtual void OnReceiveThreadException(Exception exception)
        {
            if (exception is SocketException)
            {
                if(logger.Log(LogType.Trace))
                    logger.Trace($"Ignored UDP socket receive thread exception. {exception}");
            }
            else
            {
                if (logger.Log(LogType.Fatal))
                    logger.Fatal($"UDP socket request thread exception. {exception}");

                Close();
            } 
        }

        /// <summary>
        /// On UDP socket datagram receive
        /// </summary>
        protected abstract void OnDatagramReceive(Datagram datagram);
    }
}
