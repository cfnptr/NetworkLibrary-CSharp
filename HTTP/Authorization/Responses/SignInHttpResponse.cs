using InjectorGames.SharedLibrary.Credentials;
using System;

namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Sign in HTTP response class
    /// </summary>
    public class SignInHttpResponse : BaseHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "SignIn";

        /// <summary>
        /// Response type string value
        /// </summary>
        public override string ResponseType => Type;

        /// <summary>
        /// Sign in request result
        /// </summary>
        public int result;
        /// <summary>
        /// Server version
        /// </summary>
        public Version version;
        /// <summary>
        /// Account access token
        /// </summary>
        public Token accessToken;

        /// <summary>
        /// Creates a new sign in HTTP response class instance
        /// </summary>
        public SignInHttpResponse() { }
        /// <summary>
        /// Creates a new sign in HTTP response class instance
        /// </summary>
        public SignInHttpResponse(int result, Version version = null, Token accessToken = null)
        {
            this.result = result;
            this.version = version;
            this.accessToken = accessToken;
        }
        /// <summary>
        /// Creates a new sign in HTTP response class instance
        /// </summary>
        public SignInHttpResponse(ResultType result, Version version = null, Token accessToken = null)
        {
            this.result = (int)result;
            this.version = version;
            this.accessToken = accessToken;
        }
        /// <summary>
        /// Creates a new sign up HTTP response class instance
        /// </summary>
        public SignInHttpResponse(string data)
        {
            var values = data.Split(' ');

            if (values.Length == 1)
            {
                result = int.Parse(values[0]);
            }
            else if (values.Length == 3)
            {
                result = int.Parse(values[0]);
                version = new Version(values[1]);
                accessToken = new Token(values[12]);
            }
            else
            {
                throw new ArgumentException("Incorrect data");
            }
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public override string ToBody()
        {
            if (accessToken != null)
                return $"{Type}\n{result} {version} {accessToken.ToBase64()}";
            else
                return $"{Type}\n{result}";
        }

        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType : int
        {
            BadRequest = BaseResultType.BadRequest,
            Success = BaseResultType.Success,

            IncorrectUsername,
            IncorrectPassword,
            AccountIsBlocked,
            FailedToWrite,
            Count,
        }
    }
}
