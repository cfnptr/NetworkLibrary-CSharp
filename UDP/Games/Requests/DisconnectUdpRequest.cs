using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Games.Requests
{
    /// <summary>
    /// Disconnect UDP request class
    /// </summary>
    public class DisconnectUdpRequest : IUdpRequestResponse
    {
        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public const int ByteSize = Datagram.HeaderByteSize + sizeof(int);

        /// <summary>
        /// Request byte size of the datagram data array
        /// </summary>
        public int DataByteSize => ByteSize;

        /// <summary>
        /// Disconnect reason
        /// </summary>
        public int reason;

        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public DisconnectUdpRequest() { }
        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public DisconnectUdpRequest(int reason)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new connect UDP request class instance
        /// </summary>
        public DisconnectUdpRequest(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using (var memoryStream = new MemoryStream(datagram.data))
            {
                memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(new MemoryStream()))
                    reason = binaryReader.ReadInt32();
            }
        }

        /// <summary>
        /// Converts UDP request to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];

            using (var binaryWriter = new BinaryWriter(new MemoryStream(data)))
            {
                binaryWriter.Write((byte)UdpRequestType.Disconnect);
                binaryWriter.Write(reason);
                return data;
            }
        }

        // TODO: add reason enumerator
    }
}
