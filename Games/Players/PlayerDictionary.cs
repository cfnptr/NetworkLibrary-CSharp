using InjectorGames.SharedLibrary.Credentials;
using System.Collections.Concurrent;
using System.Net;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player dictionary class
    /// </summary>
    public class PlayerDictionary<T> : ConcurrentDictionary<long, T> where T : IPlayer
    {
        /// <summary>
        /// Attempts to get player from the concurrent dictionary
        /// </summary>
        public bool TryGetValue(Username name, out T player)
        {
            foreach (var value in Values)
            {
                if (name.Equals(value.Name))
                {
                    player = value;
                    return true;
                }
            }

            player = default;
            return false;
        }
        /// <summary>
        /// Attempts to get player from the concurrent dictionary
        /// </summary>
        public bool TryGetValue(IPEndPoint remoteEndPoint, out T player)
        {
            foreach (var value in Values)
            {
                if (remoteEndPoint.Equals(value.RemoteEndPoint))
                {
                    player = value;
                    return true;
                }
            }

            player = default;
            return false;
        }
    }
}
