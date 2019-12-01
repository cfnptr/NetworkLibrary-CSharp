
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

using System.Net;

namespace OpenNetworkLibrary
{
    /// <summary>
    /// User datagram protocol IPv4 socket handler interface
    /// </summary>
    public interface IUdpSocketHandler
    {
        /// <summary>
        /// Is UDP socket handler threads still running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Binds UDP socket and starts receive thread
        /// </summary>
        void Start(IPEndPoint localEndPoint);
        /// <summary>
        /// Closes UDP socket handler socket and stops receive thread
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
    }
}
