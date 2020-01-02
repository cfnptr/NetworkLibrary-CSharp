using InjectorGames.SharedLibrary.Credentials;
using System;

namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// HTTP server sign in response class
    /// </summary>
    public class SignInResponse : IHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "SignIn";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

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
        /// Creates a new sign in response class instance
        /// </summary>
        public SignInResponse() { }
        /// <summary>
        /// Creates a new sign in response class instance
        /// </summary>
        public SignInResponse(int result, Version version = null, Token accessToken = null)
        {
            this.result = result;
            this.version = version;
            this.accessToken = accessToken;
        }
        /// <summary>
        /// Creates a new sign up response class instance
        /// </summary>
        public SignInResponse(string data)
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
        public string ToBody()
        {
            if (accessToken != null)
                return $"{Type}\n{result} {version} {accessToken.ToBase64()}";
            else
                return $"{Type}\n{result}";
        }
    }
}
