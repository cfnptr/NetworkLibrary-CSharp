namespace InjectorGames.NetworkLibrary.HTTP.Gaming.Responses
{
    /// <summary>
    /// Join room request result type
    /// </summary>
    public enum JoinRoomResultType
    {
        BadRequest = HttpResponseResultType.BadRequest,
        Success = HttpResponseResultType.Success,

        IncorrectUsername,
        IncorrectAccessToken,
        FailedToJoin,
    }
}
