using InjectorGames.NetworkLibrary.HTTP.Authorization.Requests;
using InjectorGames.NetworkLibrary.HTTP.Authorization.Responses;
using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using InjectorGames.SharedLibrary.Logs;
using System;
using System.Collections.Specialized;
using System.Net;

namespace InjectorGames.NetworkLibrary.HTTP.Authorization
{
    /// <summary>
    /// Authorization HTTP server class
    /// </summary>
    public class AuthHttpServer<TAccount, TAccountFactory> : HttpServer, IAuthHttpServer<TAccount, TAccountFactory>
        where TAccount : IAccount
        where TAccountFactory : IAccountFactory<TAccount>
    {
        /// <summary>
        /// Server version
        /// </summary>
        protected readonly Version version;
        /// <summary>
        /// Account factory
        /// </summary>
        protected readonly TAccountFactory accountFactory;
        /// <summary>
        /// Account database
        /// </summary>
        protected readonly IAccountDatabase<TAccount, TAccountFactory> accountDatabase;

        /// <summary>
        /// Server version
        /// </summary>
        public Version Version => version;
        /// <summary>
        /// Account factory
        /// </summary>
        public TAccountFactory AccountFactory => accountFactory;
        /// <summary>
        /// Account database
        /// </summary>
        public IAccountDatabase<TAccount, TAccountFactory> AccountDatabase => accountDatabase;

        /// <summary>
        /// Creates a new authorization HTTP server class instance
        /// </summary>
        public AuthHttpServer(Version version, TAccountFactory accountFactory, IAccountDatabase<TAccount, TAccountFactory> accountDatabase, ILogger logger, string address) : base(logger, address)
        {
            this.version = version ?? throw new ArgumentNullException();
            this.accountFactory = accountFactory ?? throw new ArgumentNullException();
            this.accountDatabase = accountDatabase ?? throw new ArgumentNullException();
            listener.Prefixes.Add($"{address}{SignUpRequest.Type}/");
            listener.Prefixes.Add($"{address}{SignInRequest.Type}/");
        }

        /// <summary>
        /// On HTTP server listener request receive
        /// </summary>
        protected override void OnListenerRequestReceive(HttpListenerContext context)
        {
            var httpRequest = context.Request;
            var httpResponse = context.Response;
            var urlPair = httpRequest.RawUrl.Split('?');
            var queryString = httpRequest.QueryString;

            switch (urlPair[0])
            {
                default:
                    var response = new BadRequestResponse("Unknown request type");
                    SendResponse(httpResponse, response);
                    break;
                case SignUpRequest.Type:
                    OnSignUpRequest(queryString, httpRequest, httpResponse);
                    break;
                case SignInRequest.Type:
                    OnSignInRequest(queryString, httpRequest, httpResponse);
                    break;
            }
        }
        protected void OnSignUpRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse htppResponse)
        {
            IHttpResponse response;

            try
            {
                var request = new SignUpRequest(queryString);

                if (accountDatabase.ContainsKey(request.name))
                {
                    response = new SignUpResponse((int)SignUpResultType.UsernameBusy);
                    SendResponse(htppResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign up response, username busy. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var accountData = accountFactory.Create(accountDatabase.Count, false, 0, request.name, request.passhash, request.emailAddress, Token.Create(), httpRequest.RemoteEndPoint.Address);

                if (!accountDatabase.TryAdd(accountData))
                {
                    response = new SignUpResponse((int)SignUpResultType.FailedToWrite);
                    SendResponse(htppResponse, response);

                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to add account to the database on sign up request. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                response = new SignUpResponse((int)SignUpResultType.Success);
                SendResponse(htppResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Signed up a new account. (username: {request.name}, email: {request.emailAddress}, remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
            catch
            {
                response = new SignUpResponse((int)SignUpResultType.BadRequest);
                SendResponse(htppResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad sign up request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }
        protected void OnSignInRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            IHttpResponse response;

            try
            {
                var request = new SignUpRequest(queryString);

                if (!accountDatabase.TryGetValue(request.name, accountFactory, out TAccount account))
                {
                    response = new SignInResponse((int)SignInResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, incorrect username. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.passhash != account.Passhash)
                {
                    response = new SignInResponse((int)SignInResultType.IncorrectPassword);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, incorrect password. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (account.IsBlocked)
                {
                    response = new SignInResponse((int)SignInResultType.AccountIsBlocked);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, account is blocked. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var accessToken = Token.Create();
                account.AccessToken = accessToken;
                account.LastUseIpAddress = httpRequest.RemoteEndPoint.Address;

                if (!accountDatabase.TryUpdate(account))
                {
                    response = new SignInResponse((int)SignInResultType.FailedToWrite);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to update account in the database on sign in request. (username: {request.name}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                response = new SignInResponse((int)SignInResultType.Success, version, accessToken);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Account signed in. (username: {request.name}, remoreEndPoint: {httpRequest.RemoteEndPoint})");
            }
            catch
            {
                response = new SignInResponse((int)SignInResultType.BadRequest);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad sign in request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }

        // TODO: change password
        // TODO: forgot password
        // TODO: change email address
        // TODO: validate email address on sign up
    }
}
