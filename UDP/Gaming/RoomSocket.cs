using InjectorGames.NetworkLibrary.UDP.Gaming.Requests;
using InjectorGames.NetworkLibrary.UDP.Gaming.Responses;
using InjectorGames.SharedLibrary.Credentials;
using InjectorGames.SharedLibrary.Credentials.Accounts;
using InjectorGames.SharedLibrary.Games.Players;
using InjectorGames.SharedLibrary.Games.Rooms;
using InjectorGames.SharedLibrary.Logs;
using InjectorGames.SharedLibrary.Times;
using System;

namespace InjectorGames.NetworkLibrary.UDP.Gaming
{
    /// <summary>
    /// Room UDP sockcet class
    /// </summary>
    public class RoomSocket<TPlayer, TPlayerFactory> : TaskedUdpSocket, IRoomSocket<TPlayer, TPlayerFactory>
        where TPlayer : IPlayer
        where TPlayerFactory : IPlayerFactory<TPlayer>
    {
        /// <summary>
        /// Room identifier
        /// </summary>
        protected readonly long id;
        /// <summary>
        /// Room name
        /// </summary>
        protected string name;
        /// <summary>
        /// Room maximum player count
        /// </summary>
        protected int maxPlayerCount;

        /// <summary>
        /// Room clock
        /// </summary>
        protected readonly IClock clock;
        /// <summary>
        /// Player factory
        /// </summary>
        protected readonly TPlayerFactory playerFactory;
        /// <summary>
        /// Player concurrent database
        /// </summary>
        protected readonly IPlayerDatabase<TPlayer, TPlayerFactory> playerDatabase;
        /// <summary>
        /// Player concurrent dictionary
        /// </summary>
        protected readonly PlayerDictionary<TPlayer> players;

        /// <summary>
        /// Room identifier
        /// </summary>
        public long ID
        {
            get { return id; }
            set { throw new InvalidOperationException(); }
        }
        /// <summary>
        /// Room name
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value.Replace(";", ""); }
        }
        /// <summary>
        /// Maximum room player count
        /// </summary>
        public int MaxPlayerCount
        {
            get { return maxPlayerCount; }
            set { maxPlayerCount = value > 0 ? value : throw new ArgumentException(); }
        }

        /// <summary>
        /// Current room player count
        /// </summary>
        public int PlayerCount => players.Count;

        /// <summary>
        /// Room clock
        /// </summary>
        public IClock Clock => clock;
        /// <summary>
        /// Player factory
        /// </summary>
        public TPlayerFactory PlayerFactory => playerFactory;
        /// <summary>
        /// Player database
        /// </summary>
        public IPlayerDatabase<TPlayer, TPlayerFactory> PlayerDatabase => playerDatabase;
        /// <summary>
        /// Player concurrent dictionary
        /// </summary>
        public PlayerDictionary<TPlayer> Players => players;

        /// <summary>
        /// Creates a new room UDP socket class instance
        /// </summary>
        public RoomSocket(long id, string name, int maxPlayerCount, IClock clock, TPlayerFactory playerFactory, IPlayerDatabase<TPlayer, TPlayerFactory> playerDatabase, PlayerDictionary<TPlayer> players, int maxTaskCount, ILogger logger) : base(maxTaskCount, logger)
        {
            this.id = id;

            Name = name;
            MaxPlayerCount = maxPlayerCount;

            this.clock = clock ?? throw new ArgumentNullException();
            this.playerFactory = playerFactory ?? throw new ArgumentNullException();
            this.playerDatabase = playerDatabase ?? throw new ArgumentNullException();
            this.players = players ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Returns true if room socket is equal to the object
        /// </summary>
        public override bool Equals(object obj)
        {
            return id.Equals(((IRoom)obj).ID);
        }
        /// <summary>
        /// Returns room socket hash code 
        /// </summary>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
        /// <summary>
        /// Returns room socket string
        /// </summary>
        public override string ToString()
        {
            return id.ToString();
        }

        /// <summary>
        /// Compares room socket to the object
        /// </summary>
        public int CompareTo(object obj)
        {
            return id.CompareTo(((IRoom)obj).ID);
        }
        /// <summary>
        /// Compares two room sockets
        /// </summary>
        public int CompareTo(IRoom other)
        {
            return id.CompareTo(other.ID);
        }
        /// <summary>
        /// Returns true if room sockets is equal
        /// </summary>
        public bool Equals(IRoom other)
        {
            return id.Equals(other.ID);
        }

        /// <summary>
        /// Returns true if player has joined the room
        /// </summary>
        public bool JoinPlayer(IAccount account, out RoomInfo roomInfo, out Token connectToken)
        {
            connectToken = Token.Create();
            roomInfo = new RoomInfo(id, name);

            if (!playerDatabase.TryGetValue(account.ID, playerFactory, out TPlayer player)) { }
            player = playerFactory.Create(account.ID, clock.MS, account.Name, connectToken, null);

            return players.TryAdd(player.ID, player);
        }
        /// <summary>
        /// Returns true if player has disconnected from the room
        /// </summary>
        public bool DisconnectPlayer(long id, int reason)
        {
            if (!players.TryRemove(id, out TPlayer player))
            {
                if (logger.Log(LogType.Debug))
                    logger.Debug($"Failed to remove player from the array on disconnect. (id: {player.ID}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason}, roomID: {id})");

                return false;
            }

            if (player.RemoteEndPoint != null)
            {
                var response = new DisconnectedResponse(reason);
                Send(response, player.RemoteEndPoint);

                if (!playerDatabase.TryUpdate(player))
                {
                    if (logger.Log(LogType.Error))
                        logger.Error($"Failed to update player in the database on disconnect. (id: {player.ID}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason}, rommID: {id})");
                }

                if (logger.Log(LogType.Info))
                    logger.Info($"Disconnected server player. (id: {player.ID}, remoteEndPoint: {player.RemoteEndPoint}, reason: {reason}, rommID: {id})");
            }
            else
            {
                if (logger.Log(LogType.Info))
                    logger.Info($"Disconnected server player. (id: {player.ID}, reason: {reason}, rommID: {id})");
            }

            return true;
        }

        /// <summary>
        /// On UDP socket tasked datagram receive
        /// </summary>
        protected override void OnTaskedDatagramReceive(Datagram datagram)
        {
            if (!players.TryGetValue(datagram.ipEndPoint, out TPlayer player))
            {
                if (datagram.Type == (byte)UdpRequestType.Connect)
                    OnConnectRequest(datagram);
            }
            else
            {
                switch (datagram.Type)
                {
                    default:
                        DisconnectPlayer(player.ID, (int)DisconnectReasonType.UnknownDatagram);
                        break;
                    case (byte)UdpRequestType.Connect:
                        if (logger.Log(LogType.Debug))
                            logger.Debug($"Receive second UDP room connect request. (id:{player.ID}, remoteEndPoint: {datagram.ipEndPoint}, roomId: {id})");
                        break;
                    case (byte)UdpRequestType.Disconnect:
                        DisconnectPlayer(player.ID, (int)DisconnectReasonType.Requested);
                        break;
                }
            }
        }
        protected void OnConnectRequest(Datagram datagram)
        {
            IUdpRequestResponse response;

            try
            {
                var request = new ConnectRequest(datagram);

                if (!players.TryGetValue(request.id, out TPlayer player))
                    return;

                if (player.ConnecToken != request.connectToken)
                {
                    response = new ConnectedResponse((byte)ConnectRequestResultType.IncorrectToken);
                    Send(response, datagram.ipEndPoint);

                    if (logger.Log(LogType.Debug))
                        logger.Debug($"Failed to connect room UDP socket player, incorrect token. (id: {request.id}, remoteEndPoint: {datagram.ipEndPoint}, roomID: {id})");
                }

                player.RemoteEndPoint = datagram.ipEndPoint;
                player.LastActionMS = clock.MS;
                response = new ConnectedResponse((byte)ConnectRequestResultType.Success);
                Send(response, datagram.ipEndPoint);

                if (logger.Log(LogType.Info))
                    logger.Info($"Connected a new room UDP socket player. (id: {request.id}, remoteEndPoint: {datagram.ipEndPoint}, roomID: {id})");
            }
            catch
            {
                response = new ConnectedResponse((byte)ConnectRequestResultType.BadRequest);
                Send(response, datagram.ipEndPoint);

                if (logger.Log(LogType.Debug))
                    logger.Debug($"Failed to connect room UDP socket player, bad request. (remoteEndPoint: {datagram.ipEndPoint}, roomID: {id})");
            }
        }
    }
}
