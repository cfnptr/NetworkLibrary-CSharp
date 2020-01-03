﻿namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Sign up HTTP response class
    /// </summary>
    public class SignUpHttpResponse : IHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "SignUp";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

        /// <summary>
        /// Sign up request result
        /// </summary>
        public int result;

        /// <summary>
        /// Creates a new sign up HTTP response class instance
        /// </summary>
        public SignUpHttpResponse() { }
        /// <summary>
        /// Creates a new sign up HTTP response class instance
        /// </summary>
        public SignUpHttpResponse(int result)
        {
            this.result = result;
        }
        /// <summary>
        /// Creates a new sign up HTTP response class instance
        /// </summary>
        public SignUpHttpResponse(ResultType result)
        {
            this.result = (int)result;
        }
        /// <summary>
        /// Creates a new sign up HTTP response class instance
        /// </summary>
        public SignUpHttpResponse(string data)
        {
            result = int.Parse(data);
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public string ToBody()
        {
            return $"{Type}\n{result}";
        }

        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType : int
        {
            BadRequest = HttpResponseResultType.BadRequest,
            Success = HttpResponseResultType.Success,

            UsernameBusy,
            FailedToWrite,
            Count,
        }
    }
}
