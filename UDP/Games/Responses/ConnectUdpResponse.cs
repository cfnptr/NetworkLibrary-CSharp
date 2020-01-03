using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Games.Responses
{
    /// <summary>
    /// Connect UDP response class
    /// </summary>
    public class ConnectUdpResponse : IUdpRequestResponse
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
        /// Creates a new connect UDP response class instance
        /// </summary>
        public ConnectUdpResponse() { }
        /// <summary>
        /// Creates a new connect UDP response class instance
        /// </summary>
        public ConnectUdpResponse(byte result)
        {
            this.result = result;
        }
        /// <summary>
        /// Creates a new connect UDP response class instance
        /// </summary>
        public ConnectUdpResponse(ResultType result)
        {
            this.result = (byte)result;
        }

        /// <summary>
        /// Creates a new connect UDP response class instance
        /// </summary>
        public ConnectUdpResponse(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using (var memoryStream = new MemoryStream(datagram.data))
            {
                memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream))
                    result = binaryReader.ReadByte();
            }
        }

        /// <summary>
        /// Converts UDP response to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[ByteSize];

            using (var binaryWriter = new BinaryWriter(new MemoryStream(data)))
            {
                binaryWriter.Write((byte)UdpResponseType.Connected);
                binaryWriter.Write(result);
                return data;
            }
        }

        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType
        {
            Success = UdpResponseResultType.Success,
            BadRequest = UdpResponseResultType.BadRequest,

            IncorrectToken,
            Count,
        }
    }
}
