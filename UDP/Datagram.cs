using System.Net;

namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Datagram container structure
    /// </summary>
    public struct Datagram
    {
        /// <summary>
        /// Datagram header byte size in the data array
        /// </summary>
        public const int HeaderByteSize = 1;
        /// <summary>
        /// Index of the type value in the datagram data array
        /// </summary>
        public const int HeaderTypeIndex = 0;

        /// <summary>
        /// Datagram data byte array
        /// </summary>
        public byte[] data;
        /// <summary>
        /// Datagram remote/local ip end point
        /// </summary>
        public IPEndPoint ipEndPoint;

        /// <summary>
        /// Returns datagram data byte array length
        /// </summary>
        public int Length => data.Length;

        /// <summary>
        /// Datagram first data array byte value
        /// </summary>
        public byte Type
        {
            get { return data[HeaderTypeIndex]; }
            set { data[HeaderTypeIndex] = value; }
        }

        /// <summary>
        /// Creates a new datagram structure instance
        /// </summary>
        public Datagram(byte[] data, IPEndPoint ipEndPoint)
        {
            this.data = data;
            this.ipEndPoint = ipEndPoint;
        }
    }
}
