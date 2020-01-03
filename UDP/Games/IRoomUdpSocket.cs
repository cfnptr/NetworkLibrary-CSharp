using InjectorGames.NetworkLibrary.Games.Players;
using InjectorGames.NetworkLibrary.Games.Rooms;
using InjectorGames.SharedLibrary.Times;

namespace InjectorGames.NetworkLibrary.UDP.Games
{
    /// <summary>
    /// Room UDP socket interface
    /// </summary>
    public interface IRoomUdpSocket<TPlayer, TPlayerFactory> : ITaskedUdpSocket, IRoom
        where TPlayer : IPlayer
        where TPlayerFactory : IPlayerFactory<TPlayer>
    {
        /// <summary>
        /// Room clock
        /// </summary>
        IClock Clock { get; }
        /// <summary>
        /// Player factory
        /// </summary>
        TPlayerFactory PlayerFactory { get; }
        /// <summary>
        /// Player database
        /// </summary>
        IPlayerDatabase<TPlayer, TPlayerFactory> PlayerDatabase { get; }
        /// <summary>
        /// Player concurrent dictionary
        /// </summary>
        PlayerDictionary<TPlayer> Players { get; }
    }
}
