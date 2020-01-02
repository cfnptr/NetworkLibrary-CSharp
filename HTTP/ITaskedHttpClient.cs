namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// Tasked HTTP client interface
    /// </summary>
    public interface ITaskedHttpClient
    {
        /// <summary>
        /// Starts a new web request task
        /// </summary>
        void StartRequest(string requestUri);
        /// <summary>
        /// Returns true if web request has successfully completed
        /// </summary>
        bool GetResponse(out string response);

        /// <summary>
        /// Returns true if web request is completed
        /// </summary>
        bool IsRequestCompleted();
        /// <summary>
        /// Returns true if web request is faulted
        /// </summary>
        bool IsRequestFaulted();
    }
}
