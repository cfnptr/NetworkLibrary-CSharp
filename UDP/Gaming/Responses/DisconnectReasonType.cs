namespace InjectorGames.NetworkLibrary.UDP.Gaming.Responses
{
    /// <summary>
    /// Disconnected response reason type
    /// </summary>
    public enum DisconnectReasonType : int
    {
        UnknownDatagram = 0,
        Requested = 1,
        RoomHasClosed = 2,
        RequestTimeOut = 3,
    }
}
