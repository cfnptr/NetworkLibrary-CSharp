using InjectorGames.SharedLibrary.Credentials;
using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Games.Requests
{
    /// <summary>
    /// Connect UDP request class
    /// </summary>
    public class ConnectUdpRequest : BaseUdpRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(long) + Token.ByteSize;

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public override int DataByteSize => ByteSize;

        /// <summary>
        /// Player identifier
        /// </summary>
        public long id;
        /// <summary>
        /// Connect token
        /// </summary>
        public Token connectToken;

        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public ConnectUdpRequest() { }
        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public ConnectUdpRequest(long id, Token connectToken)
        {
            this.id = id;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public ConnectUdpRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using (var memoryStream = new MemoryStream(datagram.data))
            {
                memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream))
                {
                    id = binaryReader.ReadInt64();
                    connectToken = new Token(binaryReader);
                }
            }
        }

        /// <summary>
        /// Converts UDP request to the datagram data
        /// </summary>
        public override byte[] ToData()
        {
            var data = new byte[DataByteSize];

            using (var binaryWriter = new BinaryWriter(new MemoryStream(data)))
            {
                binaryWriter.Write((byte)UdpRequestResponseType.Connect);
                binaryWriter.Write(id);
                connectToken.ToBytes(binaryWriter);
                return data;
            }
        }
    }
}
