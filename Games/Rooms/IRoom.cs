using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using InjectorGames.SharedLibrary.Entities;
using System;

namespace InjectorGames.NetworkLibrary.Games.Rooms
{
    /// <summary>
    /// Room interface
    /// </summary>
    public interface IRoom : IIdentifiable<long>, IComparable, IComparable<IRoom>, IEquatable<IRoom>
    {
        /// <summary>
        /// Room name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Maximum room player count
        /// </summary>
        int MaxPlayerCount { get; set; }

        /// <summary>
        /// Current room player count
        /// </summary>
        int PlayerCount { get; }

        /// <summary>
        /// Returns true if player has joined the room
        /// </summary>
        bool JoinPlayer(IAccount account, out RoomInfo roomInfo, out Token connectToken);

        /// <summary>
        /// Returns true if player has disconnected from the room
        /// </summary>
        bool DisconnectPlayer(long id, int reason);
    }
}
