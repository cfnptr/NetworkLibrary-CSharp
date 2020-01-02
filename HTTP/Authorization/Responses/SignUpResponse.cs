namespace InjectorGames.NetworkLibrary.HTTP.Authorization.Responses
{
    /// <summary>
    /// Sign up response class
    /// </summary>
    public class SignUpResponse : IHttpResponse
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
        /// Creates a new sign up response class instance
        /// </summary>
        public SignUpResponse() { }
        /// <summary>
        /// Creates a new sign up response class instance
        /// </summary>
        public SignUpResponse(int result)
        {
            this.result = result;
        }
        /// <summary>
        /// Creates a new sign up response class instance
        /// </summary>
        public SignUpResponse(string data)
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
    }
}
