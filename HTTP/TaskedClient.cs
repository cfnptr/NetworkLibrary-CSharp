
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
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenNetworkLibrary.HTTP
{
    /// <summary>
    /// Tasked HTTP client class
    /// </summary>
    public class TaskedClient : HttpClient, ITaskedClient
    {
        /// <summary>
        /// HTTP client get string request task
        /// </summary>
        protected Task<string> requestTask;

        /// <summary>
        /// Is request task response was returned at least once
        /// </summary>
        public bool IsResponseReturned { get; protected set; }

        /// <summary>
        /// Creates a new tasked HTTP client class instance
        /// </summary>
        public TaskedClient()
        {
            requestTask = Task.Factory.StartNew(() => { return string.Empty; });
        }
        /// <summary>
        /// Creates a new tasked HTTP client class instance
        /// </summary>
        public TaskedClient(HttpMessageHandler handler) : base(handler)
        {
            requestTask = Task.Factory.StartNew(() => { return string.Empty; });
        }
        /// <summary>
        /// Creates a new tasked HTTP client class instance
        /// </summary>
        public TaskedClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
            requestTask = Task.Factory.StartNew(() => { return string.Empty; });
        }

        /// <summary>
        /// Starts a new web request task
        /// </summary>
        public void StartRequest(string requestUri)
        {
            if (!requestTask.IsCompleted)
                throw new InvalidOperationException("Request task still running");

            IsResponseReturned = false;
            requestTask = GetStringAsync(requestUri);
        }

        /// <summary>
        /// Returns true if web request has successfully completed
        /// </summary>
        public bool GetResponse(out string response)
        {
            if (!requestTask.IsCompleted)
                throw new InvalidOperationException("Request task still running");

            if (requestTask.IsFaulted)
            {
                response = string.Empty;
                return false;
            }
            else
            {
                response = requestTask.Result;
                return true;
            }
        }

        /// <summary>
        /// Returns received HTTP server response
        /// </summary>
        public Response GetHttpResponse()
        {
            IsResponseReturned = true;

            if (GetResponse(out string response))
            {
                var pair = response.Split('\n');

                if (pair.Length == 2)
                    return new Response(true, pair[0], pair[1]);
                else
                    return new Response(false, response, response);
            }

            return new Response(false);
        }

        /// <summary>
        /// Returns true if web request is completed
        /// </summary>
        public bool IsRequestCompleted()
        {
            return requestTask.IsCompleted;
        }
        /// <summary>
        /// Returns true if web request is faulted
        /// </summary>
        public bool IsRequestFaulted()
        {
            return requestTask.IsFaulted;
        }
    }
}
