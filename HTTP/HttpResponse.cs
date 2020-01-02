namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP response container structure
    /// </summary>
    public struct HttpResponse
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
        public HttpResponse(bool status, string type, string data)
        {
            this.status = status;
            this.type = type;
            this.data = data;
        }
        /// <summary>
        /// Creates a new HTTP response structure instance
        /// </summary>
        public HttpResponse(bool status)
        {
            this.status = status;
            type = string.Empty;
            data = string.Empty;
        }
    }
}
