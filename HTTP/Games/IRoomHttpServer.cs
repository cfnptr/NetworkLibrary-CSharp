using InjectorGames.NetworkLibrary.Games.Rooms;
using InjectorGames.NetworkLibrary.HTTP.Authorization;
using InjectorGames.SharedLibrary.Credentials.Accounts;

namespace InjectorGames.NetworkLibrary.HTTP.Games
{
    /// <summary>
    /// Room HTTP server interface
    /// </summary>
    public interface IRoomHttpServer<TRoom, TAccount, TAccountFactory> : IAuthHttpServer<TAccount, TAccountFactory>
        where TRoom : IRoom
        where TAccount : IAccount
        where TAccountFactory : IAccountFactory<TAccount>
    {
        /// <summary>
        /// Room concurrent collection
        /// </summary>
        public RoomDictionary<TRoom, TAccount> Rooms { get; }
    }
}
