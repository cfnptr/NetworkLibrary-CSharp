namespace InjectorGames.NetworkLibrary.UDP
{
    /// <summary>
    /// UDP request/response class interface
    /// </summary>
    public interface IUdpRequestResponse
    {
        /// <summary>
        /// Byte size of the datagram data array
        /// </summary>
        int DataByteSize { get; }

        /// <summary>
        /// Converts UDP request/response class data to the datagram data
        /// </summary>
        byte[] ToData();
    }
}
