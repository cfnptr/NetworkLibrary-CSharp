using System;
using System.Collections.Generic;
using System.Text;

namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// Base UDP request/response abstract class
    /// </summary>
    public abstract class BaseUdpRequestResponse : IUdpRequestResponse
    {
        /// <summary>
        /// Byte size of the datagram data array
        /// </summary>
        public abstract int DataByteSize { get; }

        /// <summary>
        /// Converts UDP request/response class data to the datagram data
        /// </summary>
        public abstract byte[] ToData();
    }
}
