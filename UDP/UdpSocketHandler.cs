
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

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QuantumBranch.OpenNetworkLibrary.UDP
{
    /// <summary>
    /// User datagram protocol IPv4 socket handler class
    /// </summary>
    public abstract class UdpSocketHandler : IUdpSocketHandler
    {
        /// <summary>
        /// Maximal UDP datagram/packet size
        /// </summary>
        public const int MaxUdpSize = 65536;

        /// <summary>
        /// UDP socket handler receive socket
        /// </summary>
        protected Socket socket;

        /// <summary>
        /// UDP socket handler receive thread
        /// </summary>
        protected Thread receiveThread;

        /// <summary>
        /// Is UDP socket handler threads still running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Creates and binds UDP socket handler instance, starts receive thread
        /// </summary>
        public void Bind(IPEndPoint localEndPoint)
        {
            if (IsRunning)
                throw new InvalidOperationException("UDP socket handler already running");

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Bind(localEndPoint);

            receiveThread = new Thread(ReceiveThreadLogic) { IsBackground = true, };
            receiveThread.Start();

            IsRunning = true;
        }

        /// <summary>
        /// Closes UDP socket handler socket and stops receive thread
        /// </summary>
        public void Close()
        {
            if (!IsRunning)
                throw new InvalidOperationException("UDP socket handler is not running");

            if (socket != null)
            {
                try
                {
                    socket.Close();
                }
                catch(Exception exception)
                {
                    OnCloseException(exception);
                }

                socket = null;
            }

            if (receiveThread != null)
            {
                try
                {
                    if (receiveThread != Thread.CurrentThread)
                        receiveThread.Join();
                }
                catch (Exception exception)
                {
                    OnCloseException(exception);
                }

                receiveThread = null;
            }

            IsRunning = false;
        }

        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(byte[] buffer, int offset, int count, IPEndPoint remoteEndPoint)
        {
            return socket.SendTo(buffer, offset, count, SocketFlags.None, remoteEndPoint);
        }
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(byte[] data, IPEndPoint remoteEndPoint)
        {
            return socket.SendTo(data, 0, data.Length, SocketFlags.None, remoteEndPoint);
        }
        /// <summary>
        /// Sends datagram to the specified remote end point
        /// </summary>
        public int Send(Datagram datagram)
        {
            return socket.SendTo(datagram.data, 0, datagram.data.Length, SocketFlags.None, datagram.ipEndPoint);
        }

        /// <summary>
        /// Receive thread delegate method
        /// </summary>
        protected void ReceiveThreadLogic()
        {
            var buffer = new byte[MaxUdpSize];

            while (IsRunning)
            {
                try
                {
                    EndPoint endPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
                    var count = socket.ReceiveFrom(buffer, 0, MaxUdpSize, SocketFlags.None, ref endPoint);

                    if (count <= 0)
                        continue;

                    var data = new byte[count];
                    Buffer.BlockCopy(buffer, 0, data, 0, count);

                    var datagram = new Datagram(data, (IPEndPoint)endPoint);
                    OnDatagramReceive(datagram);
                }
                catch (SocketException socketExceotion)
                {
                    OnReceiveThreadSocketException(socketExceotion);
                }
                catch (Exception exception)
                {
                    OnReceiveThreadException(exception);
                }
            }
        }

        /// <summary>
        /// On UDP socket handler datagram receive
        /// </summary>
        protected abstract void OnDatagramReceive(Datagram datagram);
        /// <summary>
        /// On UDP socket handler close exception
        /// </summary>
        protected abstract void OnCloseException(Exception exception);
        /// <summary>
        /// On UDP socket handler receive thread exception
        /// </summary>
        protected abstract void OnReceiveThreadException(Exception exception);
        /// <summary>
        /// On UDP socket handler receive thread socket exception
        /// </summary>
        protected abstract void OnReceiveThreadSocketException(SocketException socketException);
    }
}
