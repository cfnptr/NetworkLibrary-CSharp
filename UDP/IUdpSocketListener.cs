
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
using System.Net.Sockets;

namespace QuantumBranch.OpenNetworkLibrary.UDP
{
    /// <summary>
    /// UDP socket handler listener interface
    /// </summary>
    public interface IUdpSocketListener
    {
        /// <summary>
        /// On UDP socket handler datagram receive
        /// </summary>
        void OnDatagramReceive(Datagram datagram);

        /// <summary>
        /// On UDP socket handler close exception
        /// </summary>
        void OnCloseException(Exception exception);
        /// <summary>
        /// On UDP socket handler receive thread exception
        /// </summary>
        void OnReceiveThreadException(Exception exception);
        /// <summary>
        /// On UDP socket handler receive thread socket exception
        /// </summary>
        void OnReceiveThreadSocketException(SocketException socketException);
    }
}
