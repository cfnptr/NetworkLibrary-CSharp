
// Copyright 2019 Nikita Fediuchin (QuantumBranch)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using OpenNetworkLibrary.HTTP.Authorization.Requests;
using OpenNetworkLibrary.HTTP.Authorization.Responses;
using OpenSharedLibrary.Containers;
using OpenSharedLibrary.Credentials;
using OpenSharedLibrary.Logging;
using System;
using System.Collections.Specialized;
using System.Net;

namespace OpenNetworkLibrary.HTTP.Authorization
{
    /// <summary>
    /// Authorization HTTP server class
    /// </summary>
    public class AuthServer : Server, IAuthServer
    {
        /// <summary>
        /// Account database
        /// </summary>
        protected readonly IDatabase<Username, IAccount> accountDatabase;
        /// <summary>
        /// Account factory
        /// </summary>
        protected readonly IAccountFactory accountFactory;

        /// <summary>
        /// Account database
        /// </summary>
        public IDatabase<Username, IAccount> AccountDatabase => accountDatabase;
        /// <summary>
        /// Account factory
        /// </summary>
        public IAccountFactory AccountFactory => accountFactory;

        /// <summary>
        /// Creates a new HTTP server class instance
        /// </summary>
        public AuthServer(IDatabase<Username, IAccount> accountDatabase, IAccountFactory accountFactory, ILogger logger, string address) : base(logger, address)
        {
            this.accountDatabase = accountDatabase ?? throw new ArgumentNullException();
            this.accountFactory = accountFactory ?? throw new ArgumentNullException();
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
            IResponse response;

            try
            {
                var request = new SignUpRequest(queryString);

                if (accountDatabase.Contains(request.username))
                {
                    response = new SignUpResponse((int)SignUpResultType.UsernameBusy);
                    SendResponse(htppResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign up response, username busy. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var accountData = accountFactory.Create(request.username, request.passhash, request.emailAddress, Token.Create(), false, 0, DateTime.Now, httpRequest.RemoteEndPoint.Address);

                if (!accountDatabase.Write(request.username, accountData))
                {
                    response = new SignUpResponse((int)SignUpResultType.FailedToWrite);
                    SendResponse(htppResponse, response);

                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to write account to the database on sign up request. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                response = new SignUpResponse((int)SignUpResultType.Success);
                SendResponse(htppResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Signed up a new account. (username: {request.username}, email: {request.emailAddress}, remoteEndPoint: {httpRequest.RemoteEndPoint})");
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
            IResponse response;

            try
            {
                var request = new SignUpRequest(queryString);
                var accountData = accountDatabase.Read(request.username);

                if (accountData == null)
                {
                    response = new SignInResponse((int)SignInResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, incorrect username. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.passhash != accountData.Passhash)
                {
                    response = new SignInResponse((int)SignInResultType.IncorrectPassword);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, incorrect password. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (accountData.IsBlocked)
                {
                    response = new SignInResponse((int)SignInResultType.AccountIsBlocked);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server sign in response, account is blocked. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var accessToken = Token.Create();
                accountData.DateTime = DateTime.Now;
                accountData.IpAddress = httpRequest.RemoteEndPoint.Address;
                accountData.AccessToken = accessToken;

                if (!accountDatabase.Write(request.username, accountData))
                {
                    response = new SignInResponse((int)SignInResultType.FailedToWrite);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to write account to the database on sign in request. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                response = new SignInResponse((int)SignInResultType.Success, accessToken);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Account signed in. (username: {request.username}, remoreEndPoint: {httpRequest.RemoteEndPoint})");
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
