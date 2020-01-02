using InjectorGames.SharedLibrary.Logs;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP server class
    /// </summary>
    public abstract class HttpServer : IHttpServer
    {
        /// <summary>
        /// Server logger
        /// </summary>
        protected readonly ILogger logger;
        /// <summary>
        /// Server address
        /// </summary>
        protected readonly string address;

        /// <summary>
        /// HTTP listener socket
        /// </summary>
        protected readonly HttpListener listener;
        /// <summary>
        /// HTTP request thread
        /// </summary>
        protected readonly Thread requestThread;

        /// <summary>
        /// Is server threads still running
        /// </summary>
        public bool IsRunning => listener.IsListening;

        /// <summary>
        /// Server logger
        /// </summary>
        public ILogger Logger => logger;
        /// <summary>
        /// Server address
        /// </summary>
        public string Address => address;

        /// <summary>
        /// Creates a new HTTP server class instance
        /// </summary>
        public HttpServer(ILogger logger, string address)
        {
            this.logger = logger ?? throw new ArgumentNullException();
            this.address = address ?? throw new ArgumentNullException();

            listener = new HttpListener();
            requestThread = new Thread(RequestThreadLogic) { IsBackground = true, };
        }

        /// <summary>
        /// Starts HTTP server listener and receive thread
        /// </summary>
        public void Start()
        {
            if (listener.IsListening)
            {
                if (logger.Log(LogType.Debug))
                    logger.Debug("Failed to start HTTP server, already started.");

                return;
            }


            listener.Start();
            requestThread.Start();

            if (logger.Log(LogType.Info))
                logger.Info("HTTP server started.");
        }

        /// <summary>
        /// Closes listener and stops receive thread
        /// </summary>
        public void Close()
        {
            if (!listener.IsListening)
            {
                if (logger.Log(LogType.Debug))
                    logger.Debug("Failed to close HTTP server, already closed.");

                return;
            }

            try
            {
                listener.Close();

                if (logger.Log(LogType.Debug))
                    logger.Debug("HTTP server listener closed.");
            }
            catch (Exception exception)
            {
                OnCloseException(exception);
            }

            if (logger.Log(LogType.Info))
                logger.Info("HTTP server closed.");
        }

        /// <summary>
        /// Request thread delegate method
        /// </summary>
        protected void RequestThreadLogic()
        {
            if (logger.Log(LogType.Debug))
                logger.Debug("HTTP server request thread started.");

            while (listener.IsListening)
            {
                try
                {
                    var context = listener.GetContext();

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Received HTTP server request. (remoteEndPoint: {context.Request.RemoteEndPoint}, rawUrl: {context.Request.RawUrl})");

                    OnListenerRequestReceive(context);
                }
                catch (Exception exception)
                {
                    OnRequestThreadException(exception);
                }
            }

            if (logger.Log(LogType.Debug))
                logger.Debug("HTTP server request thread stopped.");
        }

        /// <summary>
        /// On HTTP server request thread exception
        /// </summary>
        protected virtual void OnRequestThreadException(Exception exception)
        {
            if (exception is HttpListenerException)
            {
                if (logger.Log(LogType.Trace))
                    logger.Trace($"Ignored HTTP server request thread listener exception. {exception}");
            }
            else
            {
                if (logger.Log(LogType.Fatal))
                    logger.Fatal($"HTTP server request thread exception. {exception}");

                Close();
            }
        }

        /// <summary>
        /// On HTTP server close exception
        /// </summary>
        protected virtual void OnCloseException(Exception exception)
        {
            if (logger.Log(LogType.Fatal))
                logger.Fatal($"Failed to close HTTP server. {exception}");
        }

        /// <summary>
        /// On HTTP server listener request receive
        /// </summary>
        protected abstract void OnListenerRequestReceive(HttpListenerContext context);

        /// <summary>
        /// Sends HTTP response to the client
        /// </summary>
        public static void SendResponse(HttpListenerResponse httpResponse, string message, bool close)
        {
            var buffer = Encoding.UTF8.GetBytes(message.ToString());
            httpResponse.ContentLength64 = buffer.Length;

            var output = httpResponse.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            if (close)
                output.Close();
        }
        /// <summary>
        /// Sends HTTP server response to the client
        /// </summary>
        public static void SendResponse(HttpListenerResponse httpResponse, IHttpResponse response)
        {
            SendResponse(httpResponse, response.ToBody(), true);
        }
    }
}
