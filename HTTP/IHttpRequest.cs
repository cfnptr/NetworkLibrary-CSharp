namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP request interface
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        string RequestType { get; }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        string ToURL(string address);
    }
}
