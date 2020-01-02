using InjectorGames.SharedLibrary.Credentials;
using System;

namespace InjectorGames.NetworkLibrary.HTTP.Gaming.Responses
{
    /// <summary>
    /// Join room response container class
    /// </summary> 
    public class JoinRoomResponse : IHttpResponse
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
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse() { }
        /// <summary>
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse(int result, Token connectToken = null)
        {
            this.result = result;
            this.connectToken = connectToken;
        }
        /// <summary>
        /// Creates a new join room response class instance
        /// </summary>
        public JoinRoomResponse(string data)
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
    }
}
