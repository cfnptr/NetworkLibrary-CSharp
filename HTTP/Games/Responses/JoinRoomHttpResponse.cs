using InjectorGames.SharedLibrary.Credentials;
using System;

namespace InjectorGames.NetworkLibrary.HTTP.Games.Responses
{
    /// <summary>
    /// Join room HTTP response class
    /// </summary> 
    public class JoinRoomHttpResponse : IHttpResponse
    {
        /// <summary>
        /// Response type string value
        /// </summary>
        public const string Type = "JoinRoom";

        /// <summary>
        /// Response type string value
        /// </summary>
        public string ResponseType => Type;

        /// <summary>
        /// Join room request result
        /// </summary>
        public int result;
        /// <summary>
        /// Room connect token
        /// </summary>
        public Token connectToken;

        /// <summary>
        /// Creates a new join room HTTP response class instance
        /// </summary>
        public JoinRoomHttpResponse() { }
        /// <summary>
        /// Creates a new join room HTTP response class instance
        /// </summary>
        public JoinRoomHttpResponse(int result, Token connectToken = null)
        {
            this.result = result;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new join room HTTP response class instance
        /// </summary>
        public JoinRoomHttpResponse(ResultType result, Token connectToken = null)
        {
            this.result = (int)result;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new join room HTTP response class instance
        /// </summary>
        public JoinRoomHttpResponse(string data)
        {
            var values = data.Split(' ');

            if (values.Length == 1)
            {
                result = int.Parse(values[0]);
            }
            else if (values.Length == 2)
            {
                result = int.Parse(values[0]);
                connectToken = new Token(values[1]);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Returns HTTP response body
        /// </summary>
        public string ToBody()
        {
            if (connectToken != null)
                return $"{Type}\n{result} {connectToken.ToBase64()}";
            else
                return $"{Type}\n{result}";
        }

        /// <summary>
        /// Result type
        /// </summary>
        public enum ResultType
        {
            BadRequest = HttpResponseResultType.BadRequest,
            Success = HttpResponseResultType.Success,

            IncorrectUsername,
            IncorrectAccessToken,
            FailedToJoin,
            Count,
        }
    }
}
