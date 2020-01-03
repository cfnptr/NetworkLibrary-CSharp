using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Extensions;
using System.IO;
using System.Net;

namespace InjectorGames.NetworkLibrary.Games.Players
{
    /// <summary>
    /// Player class
    /// </summary>
    public class Player : IPlayer
    {
        /// <summary>
        /// Player data byte array size in bytes
        /// </summary>
        public const int ByteSize = sizeof(long) * 2 + Username.ByteSize + Token.ByteSize + IPEndPointExtension.ByteSize;

        /// <summary>
        /// Player data byte array size in bytes
        /// </summary>
        public int ByteArraySize => ByteSize;

        /// <summary>
        /// Last identifier
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// Last player action time in milliseconds
        /// </summary>
        public long LastActionMS { get; set; }
        /// <summary>
        /// Player name
        /// </summary>
        public Username Name { get; set; }
        /// <summary>
        /// Player connect token
        /// </summary>
        public Token ConnecToken { get; set; }
        /// <summary>
        /// Player remote end point
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public Player() { }
        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public Player(long id, long lastActionTime, Username name, Token connecToken, IPEndPoint remoteEndPoint)
        {
            ID = id;
            LastActionMS = lastActionTime;
            Name = name;
            ConnecToken = connecToken;
            RemoteEndPoint = remoteEndPoint;
        }
        /// <summary>
        /// Creates a new player instance
        /// </summary>
        public Player(BinaryReader binaryReader)
        {
            ID = binaryReader.ReadInt64();
            LastActionMS = binaryReader.ReadInt64();
            Name = new Username(binaryReader);
            ConnecToken = new Token(binaryReader);
            RemoteEndPoint = IPEndPointExtension.FromBytes(binaryReader);
        }

        /// <summary>
        /// Returns true if the player is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            return ID.Equals(((IPlayer)obj).ID);
        }
        /// <summary>
        /// Returns player hash code 
        /// </summary>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        /// <summary>
        /// Returns player string value
        /// </summary>
        public override string ToString()
        {
            return ID.ToString();
        }

        /// <summary>
        /// Compares player to the object
        /// </summary>
        public int CompareTo(object obj)
        {
            return ID.CompareTo(((IPlayer)obj).ID);
        }
        /// <summary>
        /// Compares two players
        /// </summary>
        public int CompareTo(IPlayer other)
        {
            return ID.CompareTo(other.ID);
        }
        /// <summary>
        /// Returns true if two players is equal
        /// </summary>
        public bool Equals(IPlayer other)
        {
            return ID.Equals(other.ID);
        }

        /// <summary>
        /// Converts player data to the byte array
        /// </summary>
        public virtual void ToBytes(BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ID);
            binaryWriter.Write(LastActionMS);
            Name.ToBytes(binaryWriter);
            ConnecToken.ToBytes(binaryWriter);
            RemoteEndPoint.ToBytes(binaryWriter);
        }
    }
}
