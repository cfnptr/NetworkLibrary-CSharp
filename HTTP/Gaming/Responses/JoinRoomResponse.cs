
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

namespace OpenNetworkLibrary.HTTP.Gaming.Responses
{
    /// <summary>
    /// Join room response container class
    /// </summary> 
    public class JoinRoomResponse : IResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "JoinRoom";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

        /// <summary>
        /// Join room request result
        /// </summary>
        public int result;
        /// <summary>
        /// Room connect token
        /// </summary>
        public Token connectToken;

        /// <summary>
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse() { }
        /// <summary>
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse(int result, Token connectToken = null)
        {
            this.result = result;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse(string data)
        {
            var values = data.Split(' ');

            if (values.Length == 1)
            {
                result = int.Parse(values[0]);
            }
            else if (values.Length == 2)
            {
                result = int.Parse(values[0]);
                connectToken = new Token(values[1]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public string ToBody()
        {
            if(connectToken != null)
                return $"{Type}\n{result} {connectToken.ToBase64()}";
            else
                return $"{Type}\n{result}";
        }
    }
}
