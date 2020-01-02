using InjectorGames.SharedLibrary.Credentials;
using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Gaming.Requests
{
    /// <summary>
    /// Connect request container class
    /// </summary>
    public class ConnectRequest : IUdpRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(long) + Token.ByteSize;

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Player identifier
        /// </summary>
        public long id;
        /// <summary>
        /// Connect token
        /// </summary>
        public Token connectToken;

        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest() { }
        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest(long id, Token connectToken)
        {
            this.id = id;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new connect request class instance
        /// </summary>
        public ConnectRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            id = binaryReader.ReadInt64();
            connectToken = new Token(binaryReader);
        }

        /// <summary>
        /// Converts request to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)UdpRequestType.Connect);
            binaryWriter.Write(id);
            connectToken.ToBytes(binaryWriter);
            return data;
        }
    }
}
