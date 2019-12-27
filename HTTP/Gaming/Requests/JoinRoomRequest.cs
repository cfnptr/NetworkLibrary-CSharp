
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
using System.Collections.Specialized;
using System.Net;

namespace OpenNetworkLibrary.HTTP.Game.Requests
{
    /// <summary>
    /// Join room request container class
    /// </summary>
    public class JoinRoomRequest : IRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        public const string Type = "/api/join-room";

        /// <summary>
        /// Request type string value
        /// </summary>
        public string RequestType => Type;

        /// <summary>
        /// Account username
        /// </summary>
        public Username username;
        /// <summary>
        /// Account access token
        /// </summary>
        public Token accessToken;
        /// <summary>
        /// Room unique identifier
        /// </summary>
        public int roomId;

        /// <summary>
        /// Creates a new join room request class instance
        /// </summary>
        public JoinRoomRequest() { }
        /// <summary>
        /// Creates a new join room request class instance
        /// </summary>
        public JoinRoomRequest(Username username, Token accessToken, int roomId)
        {
            this.username = username;
            this.accessToken = accessToken;
            this.roomId = roomId;
        }
        /// <summary>
        /// Creates a new join room request class instance
        /// </summary>
        public JoinRoomRequest(NameValueCollection queryString)
        {
            if (queryString.Count != 3)
                throw new ArgumentException();

            username = new Username(queryString.Get(0));
            accessToken = new Token(queryString.Get(1));
            roomId = int.Parse(queryString.Get(2));
        }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        public string ToURL(string address)
        {
            return $"{address}{Type}?u={username}&t={WebUtility.UrlEncode(accessToken.ToBase64())}&i={roomId}";
        }
    }
}
