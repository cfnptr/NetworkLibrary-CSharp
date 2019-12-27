
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

namespace OpenNetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// HTTP server sign in response class
    /// </summary>
    public class SignInResponse : IResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "SignIn";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

        /// <summary>
        /// Sign in request result
        /// </summary>
        public int result;
        /// <summary>
        /// Account access token
        /// </summary>
        public Token accessToken;

        /// <summary>
        /// Creates a new sign in response class instance
        /// </summary>
        public SignInResponse() { }
        /// <summary>
        /// Creates a new sign in response class instance
        /// </summary>
        public SignInResponse(int result, Token accessToken = null)
        {
            this.result = result;
            this.accessToken = accessToken;
        }
        /// <summary>
        /// Creates a new sign up response class instance
        /// </summary>
        public SignInResponse(string data)
        {
            var values = data.Split(' ');

            if (values.Length == 1)
            {
                result = int.Parse(values[0]);
            }
            else if (values.Length == 2)
            {
                result = int.Parse(values[0]);
                accessToken = new Token(values[1]);
            }
            else
            {
                throw new ArgumentException("Incorrect data");
            }
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public string ToBody()
        {
            if(accessToken != null)
                return $"{Type}\n{result} {accessToken.ToBase64()}";
            else
                return $"{Type}\n{result}";
        }
    }
}
