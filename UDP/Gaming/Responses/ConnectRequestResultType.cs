namespace InjectorGames.NetworkLibrary.UDP.Gaming.Responses
{
    /// <summary>
    /// Server connect request result type
    /// </summary>
    public enum ConnectRequestResultType
    {
        Success = UdpResponseResultType.Success,
        BadRequest = UdpResponseResultType.BadRequest,

        IncorrectToken,
    }
}
