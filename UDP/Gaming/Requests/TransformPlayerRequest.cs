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

using OpenSharedLibrary.Gaming.Players;
using System;
using System.IO;

namespace OpenNetworkLibrary.UDP.Gaming.Requests
{
    /// <summary>
    /// Transform player request container class
    /// </summary>
    public class TransformPlayerRequest : IRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + PlayerTransform.ByteSize;
        /// <summary>
        /// Minimum delay between two transform requests
        /// </summary>
        public const long MinRequestDelay = 33;

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Player transform container
        /// </summary>
        public PlayerTransform playerTransform;

        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest() { }
        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest(PlayerTransform playerTransform)
        {
            this.playerTransform = playerTransform;
        }
        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            playerTransform = new PlayerTransform(binaryReader);
        }

        /// <summary>
        /// Converts request to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)GameRequestType.TransformPlayer);
            playerTransform.ToBytes(binaryWriter);
            return data;
        }
    }
}
