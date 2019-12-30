
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
using OpenNetworkLibrary.HTTP.Gaming.Requests;
using OpenNetworkLibrary.HTTP.Gaming.Responses;
using OpenSharedLibrary.Credentials;
using OpenSharedLibrary.Credentials.Accounts;
using OpenSharedLibrary.Gaming.Rooms;
using OpenSharedLibrary.Logging;
using System;
using System.Collections.Specialized;
using System.Net;

namespace OpenNetworkLibrary.HTTP.Gaming
{
    /// <summary>
    /// Room HTTP server class
    /// </summary>
    public class RoomServer<TRoom, TAccount,TAccountFactory> : AuthServer<TAccount, TAccountFactory>, IRoomServer<TRoom, TAccount, TAccountFactory>
        where TRoom : IRoom
        where TAccount : IAccount
        where TAccountFactory : IAccountFactory<TAccount>
    {
        /// <summary>
        /// Room concurrent collection
        /// </summary>
        protected readonly RoomDictionary<TRoom, TAccount> rooms;

        /// <summary>
        /// Room concurrent collection
        /// </summary>
        public RoomDictionary<TRoom, TAccount> Rooms => rooms;

        /// <summary>
        /// Creates a new room HTTP server class instance
        /// </summary>
        public RoomServer(Version version, RoomDictionary<TRoom, TAccount> rooms, TAccountFactory accountFactory, IAccountDatabase<TAccount, TAccountFactory> accountDatabase, ILogger logger, string address) : base(version, accountFactory, accountDatabase, logger, address)
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

                if (!accountDatabase.TryGetValue(request.id, accountFactory, out TAccount account))
                {
                    response = new GetRoomInfosResponse((int)GetRoomInfosResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect id. (id: {request.id}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.accessToken != account.AccessToken)
                {
                    response = new GetRoomInfosResponse((int)GetRoomInfosResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect access token. (id: {request.id},, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var roomInfos = rooms.GetInfos();

                response = new GetRoomInfosResponse((int)GetRoomInfosResultType.Success, roomInfos);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Sended room infos to the account. (id: {request.id}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomCount: {roomInfos.Length})");
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

                if (accountDatabase.TryGetValue(request.accountId, accountFactory, out TAccount account))
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect id. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                if (request.accessToken != account.AccessToken)
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect access token. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                if (!rooms.JoinPlayer(request.roomId, account, out RoomInfo roomInfo, out Token connectToken))
                {
                    response = new JoinRoomResponse((int)JoinRoomResultType.FailedToJoin);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, failed to join. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                response = new JoinRoomResponse((int)JoinRoomResultType.Success, connectToken);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Account joined the room. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");
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
