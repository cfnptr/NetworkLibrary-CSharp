namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Sign in request result type
    /// </summary>
    public enum SignInResultType : int
    {
        BadRequest = HttpResponseResultType.BadRequest,
        Success = HttpResponseResultType.Success,

        IncorrectUsername,
        IncorrectPassword,
        AccountIsBlocked,
        FailedToWrite,
    }
}
