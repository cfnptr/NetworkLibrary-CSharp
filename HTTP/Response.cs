
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

namespace OpenNetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP response container structure
    /// </summary>
    public struct Response
    {
        /// <summary>
        /// Is response received and correct
        /// </summary>
        public bool status;
        /// <summary>
        /// Response type strign value
        /// </summary>
        public string type;
        /// <summary>
        /// Response data strign value
        /// </summary>
        public string data;

        /// <summary>
        /// Creates a new HTTP response structure instance
        /// </summary>
        public Response(bool status, string type, string data)
        {
            this.status = status;
            this.type = type;
            this.data = data;
        }
        /// <summary>
        /// Creates a new HTTP response structure instance
        /// </summary>
        public Response(bool status)
        {
            this.status = status;
            type = string.Empty;
            data = string.Empty;
        }
    }
}
