using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Gaming.Responses
{
    /// <summary>
    /// Connected response container class
    /// </summary>
    public class ConnectedResponse : IUdpRequestResponse
    {
        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(byte);

        /// <summary>
        /// Response byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Connect request result
        /// </summary>
        public byte result;

        /// <summary>
        /// Creates a new connected response class instance
        /// </summary>
        public ConnectedResponse() { }
        /// <summary>
        /// Creates a new connected response class instance
        /// </summary>
        public ConnectedResponse(byte result)
        {
            this.result = result;
        }

        /// <summary>
        /// Creates a new connected response class instance
        /// </summary>
        public ConnectedResponse(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using var memoryStream = new MemoryStream(datagram.data);
            memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            result = binaryReader.ReadByte();
        }

        /// <summary>
        /// Converts response to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[ByteSize];
            using var memoryStream = new MemoryStream(data);
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write((byte)UdpResponseType.Connected);
            binaryWriter.Write((byte)result);
            return data;
        }
    }
}
