using InjectorGames.SharedLibrary.Collections.Bytes;
using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Databases;
using System.IO;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player disk database class
    /// </summary>
    public class PlayerDiskDatabase<TPlayer, TFactory> : NameDiskDatabase<Username, long, TPlayer, TFactory>, IPlayerDatabase<TPlayer, TFactory>
        where TPlayer : IPlayer
        where TFactory : IByteArrayFactory<TPlayer>
    {
        /// <summary>
        /// Creates a new player disk database class instance
        /// </summary>
        public PlayerDiskDatabase(string path, bool useCompression) : base(path, useCompression) { }

        /// <summary>
        /// Loads database names
        /// </summary>
        public override void LoadNames()
        {
            lock (locker)
            {
                if (!File.Exists($"{path}names"))
                    return;

                var array = File.ReadAllBytes($"{path}names");
                var count = names.Count / (Username.ByteSize + sizeof(long));

                using (var binaryReader = new BinaryReader(new MemoryStream(array)))
                {
                    for (int i = 0; i < count; i++)
                    {
                        var name = new Username(binaryReader);
                        var id = binaryReader.ReadInt64();
                        names.Add(name, id);
                    }
                }
            }
        }
        /// <summary>
        /// Unloads database names
        /// </summary>
        public override void UnloadNames()
        {
            lock (locker)
            {
                var array = new byte[names.Count * (Username.ByteSize + sizeof(long))];

                using (var memoryStream = new MemoryStream(array))
                {
                    using (var binaryWriter = new BinaryWriter(memoryStream))
                    {
                        foreach (var name in names)
                        {
                            name.Key.ToBytes(binaryWriter);
                            binaryWriter.Write(name.Value);
                        }

                        using (var fileStream = new FileStream($"{path}names", FileMode.Create, FileAccess.Write))
                        {
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }
    }
}
