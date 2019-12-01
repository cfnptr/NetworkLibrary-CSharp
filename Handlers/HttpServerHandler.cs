
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
using System.Net;
using System.Threading;

namespace OpenNetworkLibrary
{
    /// <summary>
    /// Hypertext transfer protocol server handler class
    /// </summary>
    public abstract class HttpServerHandler : IHttpServerHandler
    {
        /// <summary>
        /// HTTP server handler listener socket
        /// </summary>
        protected readonly HttpListener listener;
        /// <summary>
        /// HTTP server handler request thread
        /// </summary>
        protected readonly Thread requestThread;

        /// <summary>
        /// Is HTTP socket handler threads still running
        /// </summary>
        protected bool isRunning;

        /// <summary>
        /// Is HTTP server handler threads still running
        /// </summary>
        public bool IsRunning => isRunning;

        /// <summary>
        /// Creates a new HTTP server handler class instance
        /// </summary>
        public HttpServerHandler()
        {
            listener = new HttpListener();
            requestThread = new Thread(RequestThreadLogic) { IsBackground = true, };
        }

        /// <summary>
        /// Starts HTTP server listener and receive thread
        /// </summary>
        public void Start()
        {
            if (isRunning)
                return;

            isRunning = true;

            listener.Start();
            requestThread.Start();
        }

        /// <summary>
        /// Closes HTTP server handler socket and stops receive thread
        /// </summary>
        public void Close()
        {
            if (!isRunning)
                return;

            isRunning = false;

            try
            {
                listener.Close();
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            try
            {
                if (requestThread != Thread.CurrentThread)
                    requestThread.Join();
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }
        }

        /// <summary>
        /// Request thread delegate method
        /// </summary>
        protected void RequestThreadLogic()
        {
            while (isRunning)
            {
                try
                {
                    var context = listener.GetContext();
                    OnRequestReceive(context);
                }
                catch (HttpListenerException exception)
                {
                    OnReceiveThreadHttpListenerException(exception);
                }
                catch (Exception exception)
                {
                    OnRequestThreadException(exception);
                }
            }
        }

        /// <summary>
        /// On HTTP server handler request receive
        /// </summary>
        protected abstract void OnRequestReceive(HttpListenerContext context);
        /// <summary>
        /// On HTTP server handler close exception
        /// </summary>
        protected abstract void OnCloseException(Exception exception);
        /// <summary>
        /// On HTTP server handler request thread exception
        /// </summary>
        protected abstract void OnRequestThreadException(Exception exception);
        /// <summary>
        /// On HTTP server handler request thread listener exception
        /// </summary>
        protected abstract void OnReceiveThreadHttpListenerException(HttpListenerException exception);
    }
}
