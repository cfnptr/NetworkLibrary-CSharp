
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
    /// Datagram container structure
    /// </summary>
    public struct Datagram
    {
        /// <summary>
        /// Datagram header byte size in the data array
        /// </summary>
        public const int HeaderByteSize = 1;
        /// <summary>
        /// Index of the type value in the datagram data array
        /// </summary>
        public const int HeaderTypeIndex = 0;

        /// <summary>
        /// Datagram data byte array
        /// </summary>
        public byte[] data;
        /// <summary>
        /// Datagram remote/local ip end point
        /// </summary>
        public IPEndPoint ipEndPoint;
        
        /// <summary>
        /// Returns datagram data byte array length
        /// </summary>
        public int Length => data.Length;

        /// <summary>
        /// Datagram first data array byte value
        /// </summary>
        public byte Type
        {
            get { return data[HeaderTypeIndex]; }
            set { data[HeaderTypeIndex] = value; }
        }

        /// <summary>
        /// Creates a new datagram structure instance
        /// </summary>
        public Datagram(byte[] data, IPEndPoint ipEndPoint)
        {
            this.data = data;
            this.ipEndPoint = ipEndPoint;
        }
    }
}
