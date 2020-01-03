using InjectorGames.SharedLibrary.Collections.Bytes;
using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Entities;
using System;
using System.Net;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player interface
    /// </summary>
    public interface IPlayer : IByteArray, IIdentifiable<long>, INameable<Username>, IComparable, IComparable<IPlayer>, IEquatable<IPlayer>
    {
        /// <summary>
        /// Last player action time in milliseconds
        /// </summary>
        public long LastActionMS { get; set; }
        /// <summary>
        /// Player connect token
        /// </summary>
        public Token ConnecToken { get; set; }
        /// <summary>
        /// Player remote end point
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }
    }
}
