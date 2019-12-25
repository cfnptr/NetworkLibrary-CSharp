
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

using OpenSharedLibrary.Logging;
using System;
using System.Net;
using System.Threading;

namespace OpenNetworkLibrary.HTTP
{
    /// <summary>
    /// Hypertext transfer protocol server class
    /// </summary>
    public abstract class HttpServer : IHttpServer
    {
        /// <summary>
        /// HTTP server logger
        /// </summary>
        protected readonly ILogger logger;
        /// <summary>
        /// HTTP listener socket
        /// </summary>
        protected readonly HttpListener listener;
        /// <summary>
        /// HTTP request thread
        /// </summary>
        protected readonly Thread requestThread;

        /// <summary>
        /// Is HTTP server threads still running
        /// </summary>
        public bool IsRunning => listener.IsListening;

        /// <summary>
        /// Creates a new HTTP server class instance
        /// </summary>
        public HttpServer(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException();

            listener = new HttpListener();
            requestThread = new Thread(RequestThreadLogic) { IsBackground = true, };
        }

        /// <summary>
        /// Starts HTTP server listener and receive thread
        /// </summary>
        public void Start()
        {
            listener.Start();
            requestThread.Start();

            if (logger.Log(LogType.Info))
                logger.Info("HTTP server started");
        }

        /// <summary>
        /// Closes HTTP server socket and stops receive thread
        /// </summary>
        public void Close()
        {
            try
            {
                listener.Close();

                if (logger.Log(LogType.Debug))
                    logger.Debug("HTTP server listener closed");
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            try
            {
                if (logger.Log(LogType.Debug))
                    logger.Debug("Waiting for HTTP server request thread...");

                if (requestThread != Thread.CurrentThread)
                    requestThread.Join();
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            if (logger.Log(LogType.Info))
                logger.Info("HTTP server closed");
        }

        /// <summary>
        /// Request thread delegate method
        /// </summary>
        protected void RequestThreadLogic()
        {
            if (logger.Log(LogType.Debug))
                logger.Debug("HTTP server request thread started");

            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Received HTTP server request (remoteEndPoint: {context.Request.RemoteEndPoint}, contentLength: {context.Request.ContentLength64})");

                    OnRequestReceive(context);
                }
                catch (Exception exception)
                {
                    OnRequestThreadException(exception);
                }
            }

            if (logger.Log(LogType.Debug))
                logger.Debug("HTTP server request thread stopped");
        }

        /// <summary>
        /// On HTTP server request thread exception
        /// </summary>
        protected virtual void OnRequestThreadException(Exception exception)
        {
            if (logger.Log(LogType.Fatal))
                logger.Fatal($"HTTP server request thread exception: {exception}");

            Close();
        }

        /// <summary>
        /// On HTTP server close exception
        /// </summary>
        protected virtual void OnCloseException(Exception exception)
        {
            if (logger.Log(LogType.Fatal))
                logger.Fatal($"Failed to close HTTP server: {exception}");
        }

        /// <summary>
        /// On HTTP server request receive
        /// </summary>
        protected abstract void OnRequestReceive(HttpListenerContext context);
    }
}
