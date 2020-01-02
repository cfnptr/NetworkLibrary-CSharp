using InjectorGames.SharedLibrary.Credentials;
using System;
using System.Collections.Specialized;
using System.Net;

namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Requests
{
    /// <summary>
    /// Sign in request class
    /// </summary>
    public class SignInRequest : IHttpRequest
    {
        /// <summary>
        /// Request type string value
        /// </summary>
        public const string Type = "/api/sign-in";

        /// <summary>
        /// Request type string value
        /// </summary>
        public string RequestType => Type;

        /// <summary>
        /// Account name
        /// </summary>
        public Username name;
        /// <summary>
        /// summary passhash
        /// </summary>
        public Passhash passhash;

        /// <summary>
        /// Creates a new sign in request class instance
        /// </summary>
        public SignInRequest() { }
        /// <summary>
        /// Creates a new sign in request class instance
        /// </summary>
        public SignInRequest(Username name, Passhash passhash)
        {
            this.name = name;
            this.passhash = passhash;
        }
        /// <summary>
        /// Creates a new sign in request class instance
        /// </summary>
        public SignInRequest(NameValueCollection queryString)
        {
            if (queryString.Count != 2)
                throw new ArgumentException();

            name = new Username(queryString.Get(0));
            passhash = new Passhash(queryString.Get(1));
        }

        /// <summary>
        /// Returns HTTP request URL
        /// </summary>
        public string ToURL(string address)
        {
            return $"{address}{Type}?n={name}&p={WebUtility.UrlEncode(passhash.ToBase64())}";
        }
    }
}
