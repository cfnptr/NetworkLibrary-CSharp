
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

namespace OpenNetworkLibrary
{
    /// <summary>
    /// Hypertext transfer protocol client handler interface
    /// </summary>
    public interface IHttpClientHandler
    {
        /// <summary>
        /// Starts a new web request task
        /// </summary>
        void StartRequest(string requestUri);
        /// <summary>
        /// Returns true if web request has successfully completed
        /// </summary>
        bool GetResponse(out string response);

        /// <summary>
        /// Returns true if web request is completed
        /// </summary>
        bool IsRequestCompleted();
        /// <summary>
        /// Returns true if web request is faulted
        /// </summary>
        bool IsRequestFaulted();
    }
}
