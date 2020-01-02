namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// HTTP server bad request response class
    /// </summary>
    public class BadRequestResponse : IHttpResponse
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
