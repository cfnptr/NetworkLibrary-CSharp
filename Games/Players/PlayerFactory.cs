using InjectorGames.SharedLibrary.Credentials;
using System.IO;
using System.Net;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player factory class
    /// </summary>
    public class PlayerFactory : IPlayerFactory<IPlayer>
    {
        /// <summary>
        /// Player data byte array size
        /// </summary>
        public int ByteArraySize => Player.ByteSize;

        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public IPlayer Create()
        {
            return new Player();
        }
        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public IPlayer Create(BinaryReader binaryReader)
        {
            return new Player(binaryReader);
        }
        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public IPlayer Create(long id, long lastActionTime, Username name, Token connecToken, IPEndPoint remoteEndPoint)
        {
            return new Player(id, lastActionTime, name, connecToken, remoteEndPoint);
        }
    }
}
