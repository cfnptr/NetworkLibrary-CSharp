namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Tasked UDP socket interface
    /// </summary>
    public interface ITaskedUdpSocket : IUdpSocket
    {
        /// <summary>
        /// Still active task count
        /// </summary>
        int TaskCount { get; }
    }
}
