using InjectorGames.SharedLibrary.Games.Players;
using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Gaming.Requests
{
    /// <summary>
    /// Transform player request container class
    /// </summary>
    public class TransformPlayerRequest : IUdpRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + PlayerTransform.ByteSize;
        /// <summary>
        /// Minimum delay between two transform requests
        /// </summary>
        public const long MinRequestDelay = 33;

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Player transform container
        /// </summary>
        public PlayerTransform playerTransform;

        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest() { }
        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest(PlayerTransform playerTransform)
        {
            this.playerTransform = playerTransform;
        }
        /// <summary>
        /// Creates a new transform player request class instance
        /// </summary>
        public TransformPlayerRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            playerTransform = new PlayerTransform(binaryReader);
        }

        /// <summary>
        /// Converts request to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)GameUdpRequestType.TransformPlayer);
            playerTransform.ToBytes(binaryWriter);
            return data;
        }
    }
}
