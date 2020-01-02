namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Sign up request result type
    /// </summary>
    public enum SignUpResultType : int
    {
        BadRequest = HttpResponseResultType.BadRequest,
        Success = HttpResponseResultType.Success,

        UsernameBusy,
        FailedToWrite,
    }
}
