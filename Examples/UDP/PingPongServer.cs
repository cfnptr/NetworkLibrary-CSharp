
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
using QuantumBranch.OpenNetworkLibrary.UDP;

namespace QuantumBranch.OpenNetworkLibrary.Examples.UDP
{
	/// <summary>
    /// Ping pong UDP server class
    /// </summary>
    public class PingPongServer : IUdpSocketListener
    {
        /// <summary>
        /// Server UDP socket handler
        /// </summary>
        public UdpSocketHandler socketHandler;

        /// <summary>
        /// Creates a new ping pong server instance
        /// </summary>
        public PingPongServer()
        {
            socketHandler = new UdpSocketHandler(this);

            var localEndPoint = new IPEndPoint(IPAddress.Any, IPEndPoint.MinPort);
            socketHandler.Bind(localEndPoint);
        }

        /// <summary>
        /// On UDP socket handler datagram receive
        /// </summary>
        public void OnDatagramReceive(Datagram datagram)
        {
            var type = datagram.Type;

            try
            {
                switch (type)
                {
                    default:
                        Console.WriteLine($"Received unknown datagram from the {datagram.ipEndPoint}");
                        break;
                    case (byte)DatagramType.Ping:
                        OnDatagramPing(datagram);
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed to process received datagram from the {datagram.ipEndPoint}, {exception}");
            }
        }
        protected void OnDatagramPing(Datagram datagram)
        {
            var remoteEndPoint = datagram.ipEndPoint;
            Console.WriteLine($"Received ping datagram from the {remoteEndPoint}");

            var response = new byte[] { (byte)DatagramType.Pong, };
            socketHandler.Send(response, remoteEndPoint);
        }

        /// <summary>
        /// On UDP socket handler close exception
        /// </summary>
        public void OnCloseException(Exception exception)
        {
            Console.WriteLine($"Failed to close example server UDP socket handler, {exception}");
            throw exception;
        }
        /// <summary>
        /// On UDP socket handler receive thread exception
        /// </summary>
        public void OnReceiveThreadException(Exception exception)
        {
            Console.WriteLine($"Fatal server receive thread exception, {exception}");
            throw exception;
        }
        /// <summary>
        /// On UDP socket handler receive thread socket exception
        /// </summary>
        public void OnReceiveThreadSocketException(SocketException socketException)
        {
            Console.WriteLine("Ignored receive thread socket exception");
        }
    }
}