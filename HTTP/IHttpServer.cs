using InjectorGames.SharedLibrary.Logs;

namespace InjectorGames.NetworkLibrary.HTTP
{
    /// <summary>
    /// HTTP server interface
    /// </summary>
    public interface IHttpServer
    {
        /// <summary>
        /// Is server threads still running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Server logger
        /// </summary>
        ILogger Logger { get; }
        /// <summary>
        /// Server address
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Starts listener and receive thread
        /// </summary>
        void Start();
        /// <summary>
        /// Closes socket and stops receive thread
        /// </summary>
        void Close();
    }
}
