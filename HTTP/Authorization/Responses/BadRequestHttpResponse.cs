namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Bad request HTTP response class
    /// </summary>
    public class BadRequestHttpResponse : BaseHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "BadRequest";

        /// <summary>
        /// Response type string value
        /// </summary>
        public override string ResponseType => Type;

        /// <summary>
        /// Bad request details
        /// </summary>
        public string details;

        /// <summary>
        /// Creates a new bad request HTTP response class instance
        /// </summary>
        public BadRequestHttpResponse() { }
        /// <summary>
        /// Creates a new bad request HTTP response class instance
        /// </summary>
        public BadRequestHttpResponse(string data)
        {
            details = data;
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public override string ToBody()
        {
            return $"{Type}\n{details}";
        }
    }
}
