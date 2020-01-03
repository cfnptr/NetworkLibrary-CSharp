using InjectorGames.SharedLibrary.Credentials;
using System;
using System.Collections.Specialized;
using System.Net;

namespace InjectorGames.NetworkLibrary.HTTP.Games.Requests
{
    /// <summary>
    /// Join room HTTP request class
    /// </summary>
    public class JoinRoomHttpRequest : IHttpRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        public const string Type = "/api/join-room";

        /// <summary>
        /// Request type string value
        /// </summary>
        public string RequestType => Type;

        /// <summary>
        /// Account identifier
        /// </summary>
        public long accountId;
        /// <summary>
        /// Account access token
        /// </summary>
        public Token accessToken;
        /// <summary>
        /// Room unique identifier
        /// </summary>
        public long roomId;

        /// <summary>
        /// Creates a new join room HTTP request class instance
        /// </summary>
        public JoinRoomHttpRequest() { }
        /// <summary>
        /// Creates a new join room HTTP request class instance
        /// </summary>
        public JoinRoomHttpRequest(long accountId, Token accessToken, long roomId)
        {
            this.accountId = accountId;
            this.accessToken = accessToken;
            this.roomId = roomId;
        }
        /// <summary>
        /// Creates a new join room HTTP request class instance
        /// </summary>
        public JoinRoomHttpRequest(NameValueCollection queryString)
        {
            if (queryString.Count != 3)
                throw new ArgumentException();

            accountId = long.Parse(queryString.Get(0));
            accessToken = new Token(queryString.Get(1));
            roomId = long.Parse(queryString.Get(2));
        }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        public string ToURL(string address)
        {
            return $"{address}{Type}?a={accountId}&t={WebUtility.UrlEncode(accessToken.ToBase64())}&r={roomId}";
        }
    }
}
