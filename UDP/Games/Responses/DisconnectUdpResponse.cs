using System;
using System.IO;

namespace InjectorGames.NetworkLibrary.UDP.Games.Responses
{
    /// <summary>
    /// Disconnect UDP response class
    /// </summary>
    public class DisconnectUdpResponse : IUdpRequestResponse
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
        /// Disconnect response reason type
        /// </summary>
        public int reason;

        /// <summary>
        /// Creates a new disconnect UDP response class instance
        /// </summary>
        public DisconnectUdpResponse() { }
        /// <summary>
        /// Creates a new disconnect UDP response class instance
        /// </summary>
        public DisconnectUdpResponse(int reason)
        {
            this.reason = reason;
        }
        /// <summary>
        /// Creates a new disconnect UDP response class instance
        /// </summary>
        public DisconnectUdpResponse(ReasonType reason)
        {
            this.reason = (int)reason;
        }
        /// <summary>
        /// Creates a new disconnect UDP response class instance
        /// </summary>
        public DisconnectUdpResponse(Datagram datagram)
        {
            if (datagram.Length != ByteSize)
                throw new ArgumentException();

            using (var memoryStream = new MemoryStream(datagram.data))
            {
                memoryStream.Seek(Datagram.HeaderByteSize, SeekOrigin.Begin);

                using (var binaryReader = new BinaryReader(memoryStream))
                    reason = binaryReader.ReadInt32();
            }
        }

        /// <summary>
        /// Converts UDP response to the datagram data
        /// </summary>
        public byte[] ToData()
        {
            var data = new byte[DataByteSize];

            using (var binaryWriter = new BinaryWriter(new MemoryStream(data)))
            {
                binaryWriter.Write((byte)UdpResponseType.Disconnected);
                binaryWriter.Write(reason);
                return data;
            }
        }

        /// <summary>
        /// Reason type
        /// </summary>
        public enum ReasonType : int
        {
            UnknownDatagram,
            Requested,
            RoomHasClosed,
            RequestTimeOut,
            Count,
        }
    }
}
