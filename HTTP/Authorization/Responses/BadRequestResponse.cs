
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

namespace OpenNetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// HTTP server bad request response class
    /// </summary>
    public class BadRequestResponse : IResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "BadRequest";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

        /// <summary>
        /// Bad request details
        /// </summary>
        public string details;

        /// <summary>
        /// Creates a new bad request response class instance
        /// </summary>
        public BadRequestResponse() { }
        /// <summary>
        /// Creates a new bad request response class instance
        /// </summary>
        public BadRequestResponse(string data)
        {
            details = data;
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public string ToBody()
        {
            return $"{Type}\n{details}";
        }
    }
}
