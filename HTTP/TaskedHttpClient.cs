using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// Tasked HTTP client class
    /// </summary>
    public class TaskedHttpClient : HttpClient, ITaskedHttpClient
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
        public TaskedHttpClient()
        {
            requestTask = Task.Factory.StartNew(() => { return string.Empty; });
        }
        /// <summary>
        /// Creates a new tasked HTTP client class instance
        /// </summary>
        public TaskedHttpClient(HttpMessageHandler handler) : base(handler)
        {
            requestTask = Task.Factory.StartNew(() => { return string.Empty; });
        }
        /// <summary>
        /// Creates a new tasked HTTP client class instance
        /// </summary>
        public TaskedHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
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
        public HttpResponse GetHttpResponse()
        {
            IsResponseReturned = true;

            if (GetResponse(out string response))
            {
                var pair = response.Split('\n');

                if (pair.Length == 2)
                    return new HttpResponse(true, pair[0], pair[1]);
                else
                    return new HttpResponse(false, response, response);
            }

            return new HttpResponse(false);
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
