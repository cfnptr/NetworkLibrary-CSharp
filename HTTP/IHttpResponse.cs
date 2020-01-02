namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP response interface
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        string ResponseType { get; }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        string ToBody();
    }
}
