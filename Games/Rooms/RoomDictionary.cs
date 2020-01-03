using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using System.Collections.Concurrent;

namespace InjectorGames.NetworkLibrary.Games.Rooms
{
    /// <summary>
    /// Room dictionary class
    /// </summary>
    public class RoomDictionary<TRoom, TAccount> : ConcurrentDictionary<long, TRoom> where TRoom : IRoom where TAccount : IAccount
    {
        /// <summary>
        /// Returns room informations array
        /// </summary>
        public RoomInfo[] GetInfos()
        {
            var index = 0;
            var count = Values.Count;
            var roomInfos = new RoomInfo[count];

            foreach (var room in Values)
            {
                if (index >= count)
                    break;

                roomInfos[index++] = new RoomInfo(room.ID, room.Name);
            }

            return roomInfos;
        }

        /// <summary>
        /// Returns true if player has joined the room
        /// </summary>
        public bool JoinPlayer(long roomId, TAccount account, out RoomInfo roomInfo, out Token connectToken)
        {
            if (!TryGetValue(roomId, out TRoom room))
            {
                roomInfo = default;
                connectToken = null;
                return false;
            }

            return room.JoinPlayer(account, out roomInfo, out connectToken);
        }
    }
}
