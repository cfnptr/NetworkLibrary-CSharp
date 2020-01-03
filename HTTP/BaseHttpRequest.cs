namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// Base HTTP request abstract class
    /// </summary>
    public abstract class BaseHttpRequest : IHttpRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        public abstract string RequestType { get; }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        public abstract string ToURL(string address);
    }
}
