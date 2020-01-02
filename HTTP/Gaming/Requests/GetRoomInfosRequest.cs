using InjectorGames.SharedLibrary.Credentials;
using System;
using System.Collections.Specialized;
using System.Net;

namespace InjectorGames.NetworkLibrary.HTTP.Gaming.Requests
{
    /// <summary>
    /// Get room infos request container class
    /// </summary>
    public class GetRoomInfosRequest : IHttpRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        public const string Type = "/api/get-room-infos";

        /// <summary>
        /// Request type string value
        /// </summary>
        public string RequestType => Type;

        /// <summary>
        /// Account identifier
        /// </summary>
        public long id;
        /// <summary>
        /// Account access token
        /// </summary>
        public Token accessToken;

        /// <summary>
        /// Creates a new get room infos request class instance
        /// </summary>
        public GetRoomInfosRequest() { }
        /// <summary>
        /// Creates a new get room infos request class instance
        /// </summary>
        public GetRoomInfosRequest(long id, Token accessToken)
        {
            this.id = id;
            this.accessToken = accessToken;
        }
        /// <summary>
        /// Creates a new get room infos request class instance
        /// </summary>
        public GetRoomInfosRequest(NameValueCollection queryString)
        {
            if (queryString.Count != 2)
                throw new ArgumentException();

            id = long.Parse(queryString.Get(0));
            accessToken = new Token(queryString.Get(1));
        }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        public string ToURL(string address)
        {
            return $"{address}{Type}?i={id}&t={WebUtility.UrlEncode(accessToken.ToBase64())}";
        }
    }
}
