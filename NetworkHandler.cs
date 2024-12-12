using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public class Client : Player {
        #region Client Specific Variables
        public TcpClient client;
        public NetworkStream stream;
        #endregion

        #region Client Life Handling
        public Client(TcpClient client) : base() {
            // Essential client variables
            this.client = client;
            stream = client.GetStream();

            // Username variable
            byte[] buffer = new byte[128];
            stream.Read(buffer, 0, buffer.Length);
            username = Encoding.Unicode.GetString(buffer);

            // Start handling client
            Task.Run(HandleData);
            Logging.Log($"New client: {username}");
        }

        public override bool IsAlive() {
            if (alive) {
                // Attempts to send a length of 0 byte array and if no errors then client probablyyy still connected
                try {
                    bool blockingState = client.Client.Blocking;
                    try {
                        client.Client.Blocking = false;
                        client.Client.Send(new byte[1], 0, 0);
                    }
                    catch {
                        Shutdown();
                        return false;
                    }
                    finally {
                        client.Client.Blocking = blockingState;
                    }
                }
                catch {
                    Shutdown();
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        public override void Shutdown() {
            if (alive) {
                // Shutdown Client
                alive = false;
                stream?.Close();
                client?.Close();
                NetworkHandler.RemovePlayer(this);

                Logging.Log($"Client left: {username}");
            }
        }
        #endregion

        #region Data Handling Functions
        public override void SendData(NetworkType type, byte[] data) {
            if (alive) {
                try {
                    stream.WriteByte((byte)type);
                    stream.Write(data);
                }
                catch {
                    Shutdown();
                }
            }
        }
        public bool ReadBuffer(byte[] buffer) {
            try {
                stream.Read(buffer, 0, buffer.Length);
                return true;
            }
            catch {
                Shutdown();
                return false;
            }
        }
        public int ReadByte() {
            try {
                return stream.ReadByte();
            }
            catch {
                Shutdown();
                return -1;
            }
        }
        #endregion

        #region Gameplay Functions
        public async void HandleData() {
            while (alive) {
                int dataType = ReadByte();
                switch (dataType) {
                    // hex click
                    case 200: {
                            byte[] data = new byte[3]; // type, y, x
                            if (ReadBuffer(data) && (Program.gameStatus == 2 || Program.gameStatus == 3))
                                GameHandler.PlaceSettler(this, data[0], data[1], data[2]);

                            break;
                        }


                    // no data to process :(
                    case -1: {
                            await Task.Delay(200);
                            break;
                        }


                    // bad buffer
                    default: {
                            while (ReadByte() != -1)
                                continue;

                            break;
                        }
                }
            }
        }

        // Sends display update of settlers/vilalges count
        public override void SendCounterUpdate(int type) { // 0-settler 1-village 2-both
            if (type == 0 || type == 2)
                SendData(NetworkType.CounterUpdate, [0, (byte)(settlerCount / 256), (byte)(settlerCount % 256)]);
            if (type == 1 || type == 2)
                SendData(NetworkType.CounterUpdate, [1, (byte)(villageCount / 256), (byte)(villageCount % 256)]);
        }
        // Send call to increment a statistic
        public override void SendStatistic(StatisticsType statisticType) {
            SendData(NetworkType.IncrementStatistic, [(byte)statisticType]);
        }
        #endregion
    }

    public static class NetworkHandler {
        #region Server Variables
        public static List<Player> players = new List<Player>();
        public static List<Client> waitingClients = new List<Client>();
        static TcpListener server;
        #endregion

        #region Basic Server Functionality
        public static void StartServer(int port) {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            HandleConnections();
        }
        static async void HandleConnections() {
            while (true) {
                Client client = new Client(await server.AcceptTcpClientAsync());

                // Game full (256 players) - kick client
                if (players.Count + waitingClients.Count >= 256) {
                    client.Shutdown();
                    continue;
                }
                
                // Add client depending whether currently a game is being played
                if (Program.gameStatus == 1)
                    AddPlayer(client);
                else
                    waitingClients.Add(client);

            }
        }
        #endregion

        #region Player Joining/Leaving Handling
        public static void RemovePlayer(Player player) {
            player.Shutdown();

            // Remove waiting player from join queue
            if (player is Client client && waitingClients.Contains(client))
                waitingClients.Remove(client);

            // Remove player from game
            else if (players.Contains(player)) {
                int index = players.IndexOf(player);
                SendAllPlayers(NetworkType.PlayerLeave, [(byte)index]);
                Program.form.Invoke(() => Program.form.tableLayoutPanel3.GetControlFromPosition(0, index)?.Dispose());
                players.Remove(player);
            }

            // Choose next player if mid-game && traitor's turn
            if (Program.gameStatus != 1 && GameHandler.turn == player)
                GameHandler.ChooseNextPlayer();
        }
        public static void AddPlayer(Player player) {
            players.Add(player);

            foreach (Player plr in players.ToList()) {
                // Add everyone to new player's list of players
                if (plr != player) {
                    player.SendData(NetworkType.PlayerJoin, Encoding.UTF32.GetBytes(plr.username.PadRight(32)));
                }

                // Add new player to everyones' player list
                plr.SendData(NetworkType.PlayerJoin, Encoding.UTF32.GetBytes(player.username.PadRight(32)));
            }

            player.SendStatistic(StatisticsType.ServersJoined);

            // Add to server's player list
            Program.form.Invoke(Program.form.AddPlayer, player);
        }

        public static void AddWaitingClients() {
            foreach (Client client in waitingClients) {
                AddPlayer(client);
            }

            waitingClients.Clear();
        }
        #endregion

        #region Sending Data Functions
        public static void SendAllPlayers(NetworkType type, byte[] data) {
            foreach (Player player in new List<Player>(players))
                player.SendData(type, data);
        }

        public static void SendMap() {
            // Create buffers of data to send
            List<Hexagon> resourceHexes = new List<Hexagon>();
            byte[] buffer = new byte[GameHandler.map.Length];

            int i = 0;
            foreach (Hexagon hex in GameHandler.map) {
                buffer[i++] = (byte)hex.biome;
                if (hex.resource != -1)
                    resourceHexes.Add(hex);
            }

            // Send map
            SendAllPlayers(NetworkType.SendMap, (new byte[1] { (byte)GameHandler.mapSize }).Concat(buffer).ToArray());

            // Send hex resources (if any) of each hex
            foreach (Hexagon hex in resourceHexes)
                SendHexUpdate(hex, -1);
        }
        public static void SendHexUpdate(Hexagon hex, int type) {
            if (hex != null) {
                if (type == 0 || type == 1) // settler || village
                    SendAllPlayers(NetworkType.HexUpate, [(byte)type, (byte)hex.y, (byte)hex.x, hex.settler.color.R, hex.settler.color.G, hex.settler.color.B]);
                else // any value !(settler || village)
                    SendAllPlayers(NetworkType.HexUpate, [(byte)(hex.resource + 2), (byte)hex.y, (byte)hex.x]);
            }
        }
        
        public static void SendDisaster(Disaster disaster) {
            SendAllPlayers(NetworkType.NaturalDisaster, [(byte)disaster]);
        }
        public static void SendScores(bool gameEnd) {
            GameHandler.CalculatePlayerScores();
            
            List<Player> largestScores = new List<Player>([players[0]]);
            byte[] byteScores = new byte[players.Count * 2];

            int i = 0;
            foreach (Player player in players) {
                int score = player._score;
                
                // Byte[] for sending scores over network
                byteScores[i++] = (byte)(player._score / 256);
                byteScores[i++] = (byte)(player._score % 256);

                // Find largest score
                bool isEqualOrMore = score >= largestScores[0]._score;
                if (score > largestScores[0]._score)
                    largestScores.Clear();
                if (isEqualOrMore)
                    largestScores.Add(player);
            }
 
            // Send scores
            SendAllPlayers(gameEnd ? NetworkType.EndGame : NetworkType.ClearMap, byteScores);
            
            // Statistics win/lose sending
            foreach (Player player in players) {
                if (largestScores.Contains(player))
                    player.SendStatistic(Program.gameStatus == 2 ? StatisticsType.ExplorationPhasesWon : StatisticsType.SettlementPhasesWon);
                else
                    player.SendStatistic(Program.gameStatus == 2 ? StatisticsType.ExplorationPhasesLost : StatisticsType.SettlementPhasesLost);
            }
        }
        #endregion
    }
}
