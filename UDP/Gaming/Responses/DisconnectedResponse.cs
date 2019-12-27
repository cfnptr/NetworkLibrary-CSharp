
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
using System.IO;

namespace OpenNetworkLibrary.UDP.Gaming.Responses
{
    /// <summary>
    /// Disconnected response container class
    /// </summary>
    public class DisconnectedResponse : IRequestResponse
    {
        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(int);

        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Disconnected response reason type
        /// </summary>
        public int reason;

        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse() { }
        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse(int reason)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            reason = binaryReader.ReadInt32();
        }

        /// <summary>
        /// Converts response to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)ResponseType.Disconnected);
            binaryWriter.Write(reason);
            return data;
        }
    }
}
