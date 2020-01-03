namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// Base HTTP response abstract class
    /// </summary>
    public abstract class BaseHttpResponse : IHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public abstract string ResponseType { get; }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public abstract string ToBody();

        /// <summary>
        /// Result type
        /// </summary>
        public enum BaseResultType : int
        {
            BadRequest,
            Success,
            Count,
        }
    }
}
