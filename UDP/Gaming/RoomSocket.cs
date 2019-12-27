
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

using OpenSharedLibrary.Gaming;
using OpenSharedLibrary.Credentials;
using OpenSharedLibrary.Logging;
using System;
using System.Diagnostics;
using OpenNetworkLibrary.UDP.Gaming.Responses;
using OpenNetworkLibrary.UDP.Gaming.Requests;

namespace OpenNetworkLibrary.UDP.Gaming
{
    /// <summary>
    /// Room UDP sockcet class
    /// </summary>
    public class RoomSocket : TaskedSocket, IRoomSocket
    {
        /// <summary>
        /// Room information container
        /// </summary>
        protected readonly RoomInfo info;

        /// <summary>
        /// Current room player count
        /// </summary>
        protected int playerCount;
        /// <summary>
        /// Maximum room player count
        /// </summary>
        protected int maxPlayerCount;

        /// <summary>
        /// Player database
        /// </summary>
        protected readonly IPlayerDatabase playerDatabase;
        /// <summary>
        /// Player factory
        /// </summary>
        protected readonly IPlayerFactory playerFactory;
        /// <summary>
        /// Player array
        /// </summary>
        protected readonly IPlayerArray players;

        /// <summary>
        /// Room timer
        /// </summary>
        protected readonly Stopwatch timer;

        /// <summary>
        /// Room information container
        /// </summary>
        public RoomInfo Info => info;

        /// <summary>
        /// Current room player count
        /// </summary>
        public int PlayerCount => playerCount;
        /// <summary>
        /// Maximum room player count
        /// </summary>
        public int MaxPlayerCount => maxPlayerCount;

        /// <summary>
        /// Player database
        /// </summary>
        public IPlayerDatabase PlayerDatabase => playerDatabase;
        /// <summary>
        /// Player factory
        /// </summary>
        public IPlayerFactory PlayerFactory => playerFactory;
        /// <summary>
        /// Player array
        /// </summary>
        public IPlayerArray Players => players;

        /// <summary>
        /// Room timer
        /// </summary>
        public Stopwatch Timer => timer;

        /// <summary>
        /// Creates a new room UDP socket class instance
        /// </summary>
        public RoomSocket(RoomInfo info, int maxPlayerCount, IPlayerDatabase playerDatabase, IPlayerFactory playerFactory, IPlayerArray players, Stopwatch timer, int maxTaskCount, ILogger logger) : base(maxTaskCount, logger)
        {
            this.info = info;
            this.maxPlayerCount = maxPlayerCount;

            this.playerDatabase = playerDatabase ?? throw new ArgumentNullException();
            this.playerFactory = playerFactory ?? throw new ArgumentNullException();
            this.players = players ?? throw new ArgumentNullException();

            this.timer = timer ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Returns true if room socket is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is IRoom room)
                return info.ID.Equals(room.Info.ID);
            else
                throw new ArgumentException();
        }
        /// <summary>
        /// Returns room socket hash code 
        /// </summary>
        public override int GetHashCode()
        {
            return info.ID;
        }
        /// <summary>
        /// Returns room socket string
        /// </summary>
        public override string ToString()
        {
            return info.ID.ToString();
        }

        /// <summary>
        /// Compares room socket to the object
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is IRoom room)
                return Info.ID.CompareTo(room.Info.ID);
            else
                throw new ArgumentException();
        }
        /// <summary>
        /// Compares two room sockets
        /// </summary>
        public int CompareTo(IRoom other)
        {
            return info.ID.CompareTo(other.Info.ID);
        }
        /// <summary>
        /// Returns true if room sockets is equal
        /// </summary>
        public bool Equals(IRoom other)
        {
            return info.ID.Equals(other.Info.ID);
        }

        /// <summary>
        /// Sets a new maximum room player count
        /// </summary>
        public void SetMaxPlayerCount(int count)
        {
            if (count < 0)
                throw new ArgumentException();

            maxPlayerCount = count;
        }

        /// <summary>
        /// Returns true if player has joined the room
        /// </summary>
        public bool JoinPlayer(IAccount account, out RoomInfo roomInfo, out Token connectToken)
        {
            roomInfo = info;
            connectToken = Token.Create();

            var player = playerDatabase.Read(account.Username);

            if (player == null)
                player = playerFactory.Create(account.Username, connectToken, null, timer.ElapsedMilliseconds);

            return players.Add(account.Username, player);
        }
        /// <summary>
        /// Returns true if player has disconnected from the room
        /// </summary>
        public bool DisconnectPlayer(Username username, int reason)
        {
            if (!players.Remove(username, out IPlayer player))
            {
                if (logger.Log(LogType.Trace))
                    logger.Trace($"Failed to remove player from the array on disconnect. (room: {ToString()}, username: {player.Username}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason})");

                return false;
            }

            if (player.RemoteEndPoint != null)
            {
                var response = new DisconnectedResponse(reason);
                Send(response, player.RemoteEndPoint);

                if (!playerDatabase.Write(player.Username, player))
                {
                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to write player to the database on disconnect. (username: {player.Username}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason}, roomInfo: {info})");
                }

                if (logger.Log(LogType.Info))
                    logger.Info($"Disconnected server player. (username: {player.Username}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason}, roomInfo: {info})");
            }
            else
            {
                if (logger.Log(LogType.Info))
                    logger.Info($"Disconnected server player. (username: {player.Username}, reason: {reason}, roomInfo: {info})");
            }

            return true;
        }

        /// <summary>
        /// Closes room
        /// </summary>
        public virtual void CloseRoom()
        {
            Close();
        }

        /// <summary>
        /// On UDP socket tasked datagram receive
        /// </summary>
        protected override void OnTaskedDatagramReceive(Datagram datagram)
        {
            IPlayer player = players.Get(datagram.ipEndPoint);

            if (player == null)
            {
                if (datagram.Type == (byte)RequestType.Connect)
                    OnConnectRequest(datagram);
            }
            else
            {
                switch (datagram.Type)
                {
                    default:
                        DisconnectPlayer(player.Username, (int)DisconnectReasonType.UnknownDatagram);
                        break;
                    case (byte)RequestType.Connect:
                        if (logger.Log(LogType.Trace))
                            logger.Trace($"Receive second UDP room connect request. (remoteEndPoint: {datagram.ipEndPoint}, roomInfo: {info})");
                        break;
                    case (byte)RequestType.Disconnect:
                        DisconnectPlayer(player.Username, (int)DisconnectReasonType.Requested);
                        break;
                }
            }
        }
        protected void OnConnectRequest(Datagram datagram)
        {
            IRequestResponse response;

            try
            {
                var request = new ConnectRequest(datagram);
                var player = players.Get(request.username);

                if (player == null)
                    return;

                if (player.ConnecToken != request.connectToken)
                {
                    response = new ConnectedResponse((byte)ConnectRequestResultType.IncorrectToken);
                    Send(response, datagram.ipEndPoint);

                    if (logger.Log(LogType.Info))
                        logger.Info($"Failed to connect room UDP socket player, incorrect token. (username: {request.username}, remoteEndPoint: {datagram.ipEndPoint}, roomInfo: {info})");
                }

                player.RemoteEndPoint = datagram.ipEndPoint;
                player.LastActionTime = timer.ElapsedMilliseconds;
                response = new ConnectedResponse((byte)ConnectRequestResultType.Success);
                Send(response, datagram.ipEndPoint);

                if (logger.Log(LogType.Info))
                    logger.Info($"Connected a new room UDP socket player. (username: {request.username}, remoteEndPoint: {datagram.ipEndPoint}, roomInfo: {info})");
            }
            catch
            {
                response = new ConnectedResponse((byte)ConnectRequestResultType.BadRequest);
                Send(response, datagram.ipEndPoint);

                if (logger.Log(LogType.Info))
                    logger.Info($"Failed to connect room UDP socket player, bad request. (remoteEndPoint: {datagram.ipEndPoint}, roomInfo: {info})");
            }
        }
    }
}
