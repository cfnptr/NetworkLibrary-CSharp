using InjectorGames.NetworkLibrary.Games.Rooms;
using InjectorGames.NetworkLibrary.HTTP.Authorization;
using InjectorGames.NetworkLibrary.HTTP.Authorization.Requests;
using InjectorGames.NetworkLibrary.HTTP.Authorization.Responses;
using InjectorGames.NetworkLibrary.HTTP.Games.Requests;
using InjectorGames.NetworkLibrary.HTTP.Games.Responses;
using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using InjectorGames.SharedLibrary.Logs;
using System;
using System.Collections.Specialized;
using System.Net;

namespace InjectorGames.NetworkLibrary.HTTP.Games
{
    /// <summary>
    /// Room HTTP server class
    /// </summary>
    public class RoomHttpServer<TRoom, TAccount, TAccountFactory> : AuthHttpServer<TAccount, TAccountFactory>, IRoomHttpServer<TRoom, TAccount, TAccountFactory>
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
        public RoomHttpServer(Version version, RoomDictionary<TRoom, TAccount> rooms, TAccountFactory accountFactory, IAccountDatabase<TAccount, TAccountFactory> accountDatabase, ILogger logger, string address) : base(version, accountFactory, accountDatabase, logger, address)
        {
            this.rooms = rooms ?? throw new ArgumentNullException();
            listener.Prefixes.Add($"{address}{GetRoomInfosHttpRequest.Type}/");
            listener.Prefixes.Add($"{address}{JoinRoomHttpRequest.Type}/");
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
                    var response = new BadRequestHttpResponse("Unknown request type");
                    SendResponse(httpResponse, response);
                    break;
                case SignUpHttpRequest.Type:
                    OnSignUpRequest(queryString, httpRequest, httpResponse);
                    break;
                case SignInHttpRequest.Type:
                    OnSignInRequest(queryString, httpRequest, httpResponse);
                    break;
                case GetRoomInfosHttpRequest.Type:
                    OnGetRoomInfosRequest(queryString, httpRequest, httpResponse);
                    break;
                case JoinRoomHttpRequest.Type:
                    OnJoinRoomRequest(queryString, httpRequest, httpResponse);
                    break;
            }
        }
        protected void OnGetRoomInfosRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            IHttpResponse response;

            try
            {
                var request = new GetRoomInfosHttpRequest(queryString);

                if (!accountDatabase.TryGetValue(request.id, accountFactory, out TAccount account))
                {
                    response = new GetRoomInfosHttpResponse((int)GetRoomInfosHttpResponse.ResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect id. (id: {request.id}, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                if (request.accessToken != account.AccessToken)
                {
                    response = new GetRoomInfosHttpResponse((int)GetRoomInfosHttpResponse.ResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server get room infos response, incorrect access token. (id: {request.id},, remoteEndPoint: {httpRequest.RemoteEndPoint})");

                    return;
                }

                var roomInfos = rooms.GetInfos();

                response = new GetRoomInfosHttpResponse((int)GetRoomInfosHttpResponse.ResultType.Success, roomInfos);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Sended room infos to the account. (id: {request.id}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomCount: {roomInfos.Length})");
            }
            catch
            {
                response = new GetRoomInfosHttpResponse((int)GetRoomInfosHttpResponse.ResultType.BadRequest);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad get room infos request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }
        protected void OnJoinRoomRequest(NameValueCollection queryString, HttpListenerRequest httpRequest, HttpListenerResponse httpResponse)
        {
            IHttpResponse response;

            try
            {
                var request = new JoinRoomHttpRequest(queryString);

                if (accountDatabase.TryGetValue(request.accountId, accountFactory, out TAccount account))
                {
                    response = new JoinRoomHttpResponse((int)JoinRoomHttpResponse.ResultType.IncorrectUsername);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect id. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                if (request.accessToken != account.AccessToken)
                {
                    response = new JoinRoomHttpResponse((int)JoinRoomHttpResponse.ResultType.IncorrectAccessToken);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, incorrect access token. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                if (!rooms.JoinPlayer(request.roomId, account, out RoomInfo roomInfo, out Token connectToken))
                {
                    response = new JoinRoomHttpResponse((int)JoinRoomHttpResponse.ResultType.FailedToJoin);
                    SendResponse(httpResponse, response);

                    if (logger.Log(LogType.Trace))
                        logger.Trace($"Sended HTTP server join room response, failed to join. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");

                    return;
                }

                response = new JoinRoomHttpResponse((int)JoinRoomHttpResponse.ResultType.Success, connectToken);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Info))
                    logger.Info($"Account joined the room. (id: {request.accountId}, remoteEndPoint: {httpRequest.RemoteEndPoint}, roomId: {request.roomId})");
            }
            catch
            {
                response = new JoinRoomHttpResponse((int)JoinRoomHttpResponse.ResultType.BadRequest);
                SendResponse(httpResponse, response);

                if (logger.Log(LogType.Trace))
                    logger.Trace($"Bad join room request. (remoteEndPoint: {httpRequest.RemoteEndPoint})");
            }
        }
    }
}
