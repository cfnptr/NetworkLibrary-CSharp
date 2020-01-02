namespace InjectorGames.NetworkLibrary.UDP.Gaming
{
    /// <summary>
    /// Game UDP request type
    /// </summary>
    public enum GameUdpRequestType : byte
    {
        TransformPlayer = UdpRequestType.Disconnect + 1,
    }
}
