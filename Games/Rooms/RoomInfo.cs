using System;

namespace InjectorGames.NetworkLibrary.Games.Rooms
{
    /// <summary>
    /// Room information container structure
    /// </summary>
    public struct RoomInfo
    {
        /// <summary>
        /// Unique room identifier
        /// </summary>
        public long id;
        /// <summary>
        /// Room name
        /// </summary>
        public string name;

        /// <summary>
        /// Creates a new room container instance
        /// </summary>
        public RoomInfo(long id, string name)
        {
            this.id = id;
            this.name = name;
        }
        /// <summary>
        /// Creates a new room container instance
        /// </summary>
        public RoomInfo(string serialized)
        {
            // TODO: compress name to the base64
            var values = serialized.Split(':');

            if (values.Length != 2)
                throw new ArgumentException();

            id = long.Parse(values[0]);
            name = values[1];
        }

        /// <summary>
        /// Returns room information string
        /// </summary>
        public override string ToString()
        {
            return $"{id}:{name}";
        }

        /// <summary>
        /// Serializes room information container
        /// </summary>
        public string Serialize()
        {
            return $"{id}:{name}";
        }
    }
}
