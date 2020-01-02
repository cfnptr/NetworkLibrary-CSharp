using InjectorGames.NetworkLibrary.HTTP.Authorization;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using InjectorGames.SharedLibrary.Games.Rooms;

namespace InjectorGames.NetworkLibrary.HTTP.Gaming
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
