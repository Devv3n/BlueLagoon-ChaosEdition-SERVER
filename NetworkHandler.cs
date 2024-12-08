﻿using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public class Client {
        #region Variables
        public static int defaultSettlerCount;

        public bool alive = true;
        public TcpClient client;
        public NetworkStream stream;
        public string username;
        public Color color;

        public int[] resourceCount = new int[6];
        public int settlerCount = defaultSettlerCount;
        public int villageCount = 3;
        public bool villagePlaced = false;

        // temp variables for end score calculating
        public int _score = 0;
        public int _islandSettlerCount = 0;
        public bool[] _uniqueIslands = new bool[8];
        public int _linkedIslands = 0;
        #endregion

        #region Client Life Handling
        public Client(TcpClient client) {
            // Essential client variables
            this.client = client;
            stream = client.GetStream();

            // Username variable
            byte[] buffer = new byte[128];
            stream.Read(buffer, 0, buffer.Length);
            username = Encoding.Unicode.GetString(buffer);

            // Color variable
            Random rng = new Random();
            color = Color.FromArgb(255, rng.Next(256), rng.Next(256), rng.Next(256));

            // Start handling client
            Task.Run(HandleData);
            Logging.Log($"New client \"{username}\"");
        }

        public bool IsAlive() {
            if (alive) {
                // Attempts to send a length of 0 byte array and if no errors then client probablyyy still connected
                try {
                    bool blockingState = client.Client.Blocking;
                    try {
                        client.Client.Blocking = false;
                        client.Client.Send(new byte[1], 0, 0);
                    }
                    catch {
                        CloseClient();
                        return false;
                    }
                    finally {
                        client.Client.Blocking = blockingState;
                    }
                }
                catch {
                    CloseClient();
                    return false;
                }

                return true;
            }
            else
                return false;
        }

        public void CloseClient() {
            if (alive) {
                alive = false;
                stream?.Close();
                client?.Close();
                NetworkHandler.RemoveClient(this);

                if (Program.form.gameStatus != 1 && GameHandler.turn == this)
                    GameHandler.ChooseNextPlayer();

                Logging.Log($"Client \'{username}\' left");
            }
        }
        #endregion

        #region Data Handling Functions
        public void SendData(byte type, byte[] data) {
            if (alive) {
                try {
                    stream.WriteByte(type);
                    stream.Write(data);
                }
                catch {
                    CloseClient();
                }
            }
        }
        void ReadBuffer(byte[] buffer) {
            try {
                stream.Read(buffer, 0, buffer.Length);
            }
            catch {
                CloseClient();
            }
        }
        int ReadByte() {
            try {
                return stream.ReadByte();
            }
            catch {
                CloseClient();
                return -1;
            }
        }
        #endregion

        #region Gameplay Functions
        async void HandleData() {
            while (alive) {
                int dataType = ReadByte();
                switch (dataType) {
                    // hex click
                    case 200: {
                            byte[] data = new byte[3]; // type, y, x
                            ReadBuffer(data);
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
        public void Reset() {
            resourceCount = new int[6];
            settlerCount = defaultSettlerCount;
            villageCount = 3;
        }
        #endregion
    }

    public static class NetworkHandler {
        #region Variables
        public static List<Client> clients = new List<Client>();
        static List<Client> waitingClients = new List<Client>();
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

                if (clients.Count + waitingClients.Count >= 256) {
                    client.CloseClient();
                    continue;
                }

                if (Program.form.gameStatus == 1)
                    AddClient(client);
                else
                    waitingClients.Add(client);

            }
        }
        #endregion

        #region Client Joining/Leaving Handling
        public static void RemoveClient(Client client) {
            client.CloseClient();

            if (waitingClients.Contains(client))
                waitingClients.Remove(client);

            else if (clients.Contains(client)) {
                int index = clients.IndexOf(client);
                SendAllClients(221, [(byte)index]);
                Program.form.Invoke(Program.form.tableLayoutPanel3.GetControlFromPosition(0, index).Dispose);
                clients.Remove(client);
            }
        }
        static void AddClient(Client client) {
            clients.Add(client);
            foreach (Client c in clients) {
                if (c != client) {
                    client.SendData(220, Encoding.Unicode.GetBytes(c.username));
                }
                c.SendData(220, Encoding.Unicode.GetBytes(client.username));
            }

            Label lbl = new Label();
            lbl.Text = client.username;
            Program.form.Invoke(() => Program.form.tableLayoutPanel3.Controls.Add(lbl));

            client.SendData(240, [2]); // Servers joined tatistic
        }
        public static void AddWaitingClients() {
            foreach (Client client in waitingClients) {
                AddClient(client);
            }
            waitingClients.Clear();
        }
        #endregion

        #region Sending Data Functions
        public static void SendAllClients(byte type, byte[] data) {
            foreach (Client client in clients)
                client.SendData(type, data);
        }

        public static void SendMap() {
            List<Hexagon> resourceHexes = new List<Hexagon>();
            byte[] buffer = new byte[GameHandler.map.Length];

            int i = 0;
            foreach (Hexagon hex in GameHandler.map) {
                buffer[i++] = (byte)hex.biome;
                if (hex.resource != -1)
                    resourceHexes.Add(hex);
            }

            foreach (Client client in clients)
                client.SendData(210, (new byte[1] { (byte)GameHandler.mapSize }).Concat(buffer).ToArray());

            foreach (Hexagon hex in resourceHexes)
                SendHexUpdate(hex, -1);
        }
        public static void SendHexUpdate(Hexagon hex, int type) {
            if (hex != null) {
                if (type == 0 || type == 1) // settler || village
                    SendAllClients(211, [(byte)type, (byte)hex.y, (byte)hex.x, hex.settler.color.R, hex.settler.color.G, hex.settler.color.B]);
                else // any value not settler || village
                    SendAllClients(211, [(byte)(hex.resource + 2), (byte)hex.y, (byte)hex.x]);
            }
        }

        public static void SendScores() {
            int[] scores = GameHandler.GetPlayerScores();

            // Send scores
            byte[] byteScores = new byte[clients.Count * 2];
            int i = 0;
            foreach (Client client in NetworkHandler.clients) {
                scores[i] = (byte)(client._score / 256);
                scores[i++] = (byte)(client._score % 256);
            }
            SendAllClients(212, byteScores);

            // Find largest scores
            i = 0;
            List<int> largestScores = new List<int>(scores[0]);
            foreach (int score in scores) {
                if (score > largestScores[0])
                    largestScores.Clear();
                if (score >= largestScores[0])
                    largestScores.Add(i);

                i++;
            }

            // Statistics win/lose sending
            i = 0;
            foreach (Client client in clients) {
                if (largestScores.Contains(i++))
                    client.SendData(240, [(byte)(Program.form.gameStatus == 2 ? 4 : 6)]); // win
                else
                    client.SendData(240, [(byte)(Program.form.gameStatus == 2 ? 5 : 7)]); // lose
            }
        }
        #endregion
    }
}