using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Gaming.Responses
{
    /// <summary>
    /// Disconnected response container class
    /// </summary>
    public class DisconnectedResponse : IUdpRequestResponse
    {
        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(int);

        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Disconnected response reason type
        /// </summary>
        public int reason;

        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse() { }
        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse(int reason)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new disconnected response class instance
        /// </summary>
        public DisconnectedResponse(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            reason = binaryReader.ReadInt32();
        }

        /// <summary>
        /// Converts response to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)UdpResponseType.Disconnected);
            binaryWriter.Write(reason);
            return data;
        }
    }
}
