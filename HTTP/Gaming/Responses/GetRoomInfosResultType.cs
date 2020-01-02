namespace InjectorGames.NetworkLibrary.HTTP.Gaming.Responses
{
    /// <summary>
    /// Gt room infos request result type
    /// </summary>
    public enum GetRoomInfosResultType
    {
        BadRequest = HttpResponseResultType.BadRequest,
        Success = HttpResponseResultType.Success,

        IncorrectUsername,
        IncorrectAccessToken,
    }
}
