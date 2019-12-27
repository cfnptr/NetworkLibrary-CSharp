
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

using OpenSharedLibrary.Credentials;
using System;
using System.IO;

namespace OpenNetworkLibrary.UDP.Gaming.Requests
{
    /// <summary>
    /// Connect request container class
    /// </summary>
    public class ConnectRequest : IRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + Username.ByteSize + Token.ByteSize;

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Player username
        /// </summary>
        public Username username;
        /// <summary>
        /// Connect token
        /// </summary>
        public Token connectToken;

        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest() { }
        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest(Username username, Token connectToken)
        {
            this.username = username;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            username = new Username(binaryReader);
            connectToken = new Token(binaryReader);
        }

        /// <summary>
        /// Converts request to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)RequestType.Connect);
            username.ToBytes(binaryWriter);
            connectToken.ToBytes(binaryWriter);
            return data;
        }
    }
}
