
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

using OpenNetworkLibrary.HTTP.Authorization;
using OpenNetworkLibrary.HTTP.Authorization.Requests;
using OpenNetworkLibrary.HTTP.Authorization.Responses;
using OpenNetworkLibrary.HTTP.Game.Requests;
using OpenNetworkLibrary.HTTP.Game.Responses;
using OpenSharedLibrary.Containers;
using OpenSharedLibrary.Credentials;
using OpenSharedLibrary.Gaming;
using OpenSharedLibrary.Logging;
using System;
using System.Collections.Specialized;
using System.Net;

namespace OpenNetworkLibrary.HTTP.Game
{
    /// <summary>
    /// Room HTTP server class
    /// </summary>
    public class RoomServer : AuthServer, IRoomServer
    {
        /// <summary>
        /// HTTP server room array
        /// </summary>
        protected readonly IRoomArray rooms;

        /// <summary>
        /// HTTP server room array
        /// </summary>
        public IRoomArray Rooms => rooms;

        /// <summary>
        /// Creates a new room HTTP server class instance
        /// </summary>
        public RoomServer(IRoomArray rooms, IDatabase<Username, IAccount> accountDatabase, IAccountFactory accountFactory, ILogger logger, string address) : base(accountDatabase, accountFactory, logger, address)
        {
            this.rooms = rooms ?? throw new ArgumentNullException();
            listener.Prefixes.Add($"{address}{GetRoomInfosRequest.Type}/");
            listener.Prefixes.Add($"{address}{JoinRoomRequest.Type}/");
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
                case GetRoomInfosRequest.Type:
                    OnGetRoomInfosRequest(queryString, httpRequest, httpResponse);
                    break;
                case JoinRoomRequest.Type:
                    OnJoinRoomRequest(queryString, httpRequest, httpResponse);
                    break;
            }
        }
        protected void OnGetRoomInfosRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            IResponse response;

            try
            {
                var request = new GetRoomInfosRequest(queryString);
                var accountData = accountDatabase.Read(request.username);

                if (accountData == null)
                {
                    response = new GetRoomInfosResponse((int)GetRoomInfosResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect username. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.accessToken != accountData.AccessToken)
                {
                    response = new GetRoomInfosResponse((int)GetRoomInfosResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect access token. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var roomInfos = rooms.GetInfos();

                response = new GetRoomInfosResponse((int)GetRoomInfosResultType.Success, roomInfos);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Sended room infos to the account. (roomCount: {roomInfos.Length} username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
            catch
            {
                response = new SignInResponse((int)SignInResultType.BadRequest);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad get room infos request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }
        protected void OnJoinRoomRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            IResponse response;

            try
            {
                var request = new JoinRoomRequest(queryString);
                var accountData = accountDatabase.Read(request.username);

                if (accountData == null)
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect username. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.accessToken != accountData.AccessToken)
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect access token. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (!rooms.JoinPlayer(request.roomId, accountData, out RoomInfo roomInfo, out Token connectToken))
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.FailedToJoin);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, failed to join. (username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                response = new JoinRoomResponse((int)JoinRoomResultType.Success, roomInfo.Version, connectToken);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Account joined the room. (roomId: {request.roomId} username: {request.username}, remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
            catch
            {
                response = new SignInResponse((int)SignInResultType.BadRequest);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad join room request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }
    }
}
