using InjectorGames.SharedLibrary.Collections.Bytes;
using InjectorGames.SharedLibrary.Credentials;
using System.Net;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player factory interface
    /// </summary>
    public interface IPlayerFactory<T> : IByteArrayFactory<T> where T : IPlayer
    {
        /// <summary>
        /// Creates a new player instance
        /// </summary>
        T Create(long id, long lastActionTime, Username name, Token connecToken, IPEndPoint remoteEndPoint);
    }
}
