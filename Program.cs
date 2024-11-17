using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    internal static class Program {
        public static Server form = new Server();

        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //yeah yeah no one cares
            //- DevvEn

            ApplicationConfiguration.Initialize();
            MapHandler.noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);

            Application.Run(form);
        }
    }

    public class Client {
        public TcpClient client;
        public NetworkStream stream;
        public string username;

        public Client(TcpClient client) {
            this.client = client;
            stream = client.GetStream();

            byte[] buffer = new byte[128];
            stream.Read(buffer, 0, buffer.Length);
            username = Encoding.Unicode.GetString(buffer);
        }

        public bool SendData(Byte[] data) {
            try {
                stream.Write(data);
                return true;
            }
            catch {
                stream.Close();
                client.Close();
                return false;
            }
        }
    }

    public static class NetworkHandler {
        static List<Client> clients = new List<Client>();
        static TcpListener server;

        public static void StartServer(int port) {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            HandleConnections();
        }

        static async void HandleConnections() {
            while (true) {
                Client client = new Client(await server.AcceptTcpClientAsync());
                clients.Add(client);

                Label lbl = new Label();
                lbl.Text = client.username;
                Program.form.tableLayoutPanel3.Controls.Add(lbl);
            }
        }

        static void CheckAlive() {
            List<int> deadClients = new List<int>();
            byte[] tmp = new byte[1];
            int i = 0;

            foreach (Client client in clients) {
                bool blockingState = client.client.Client.Blocking;
                try {
                    client.client.Client.Blocking = false;
                    client.client.Client.Send(tmp, 0, 0);
                }
                catch {
                    deadClients.Add(i);
                }
                finally {
                    client.client.Client.Blocking = blockingState;
                }
                i++;
            }

            i = 0;
            foreach (int client in deadClients) {
                Program.form.tableLayoutPanel3.Controls.Remove(Program.form.tableLayoutPanel3.GetControlFromPosition(0, client - i));
                clients.RemoveAt(client - i++);
            }
        }

        public static void SendMap() {
            byte[] buffer = new Byte[MapHandler.GetMapByteSize()];
            int i = 0;
            for (int y = 0; y < MapHandler.map.GetLength(0); y++) {
                for (int x = 0; x < MapHandler.map.GetLength(1); x++) {
                    buffer[i++] = (byte)MapHandler.map[y, x];
                }
            }

            CheckAlive();
            foreach (Client client in clients)
                client.SendData(buffer);
        }
    }

    public static class MapHandler {
        static readonly int[][] hexOffsets0 = [[-1, 0], [-1, 1], [0, 1], [1, 1], [1, 0], [0, -1]];
        static readonly int[][] hexOffsets1 = [[-1, -1], [-1, 0], [0, 1], [1, 0], [1, -1], [0, -1]];
        public const int mapSize = 16;

        public static FastNoiseLite noise = new FastNoiseLite();
        static Random random = new Random();

        public static int[,] map = new int[mapSize, mapSize];
        static List<List<int>>? islands;

        static void GenerateMap() {
            int xOffset = random.Next(-10000, 10000);
            int yOffset = random.Next(-10000, 10000);

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    map[y, x] = noise.GetNoise(x * 15 + xOffset, y * 15 + yOffset) > 0.05 ? 1 : 0;
                }
            }
        }

        static List<List<int>> DetectIslands() {
            bool[,] searched = new bool[mapSize, mapSize];
            islands = new List<List<int>>();

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (searched[y, x] || map[y, x] == 0) {
                        searched[y, x] = true;
                        continue;
                    }

                    List<int> islandPos = new List<int>();

                    void SearchNearby(int y, int x) {
                        if (0 <= y && y < mapSize && 0 <= x && x < mapSize && !searched[y, x]) {
                            searched[y, x] = true;
                            if (map[y, x] == 0)
                                return;

                            islandPos.Add(y * mapSize + x);
                            foreach (int[] o in (y % 2 == 0 ? hexOffsets0 : hexOffsets1))
                                SearchNearby(y + o[0], x + o[1]);
                        }
                    }

                    SearchNearby(y, x);
                    islands.Add(islandPos);

                }
            }

            return islands;
        }

        static bool ValidateIslands(List<List<int>> islands) {
            int bigIslandCount = 0;
            List<List<int>> removeIslands = new List<List<int>>();

            foreach (List<int> island in islands) {
                if (island.Count > 6)
                    bigIslandCount++;
                else {
                    foreach (int hex in island)
                        map[hex / mapSize, hex % mapSize] = 0;
                    removeIslands.Add(island);
                }
            }

            foreach (List<int> island in removeIslands)
                islands.Remove(island);

            if (bigIslandCount == 8 && islands.Count == 8)
                return true;
            return false;
        }

        static void SetIslandBiomes(List<List<int>> islands) {
            foreach (List<int> island in islands) {
                int biome = random.Next(1, 4);

                foreach (int hex in island)
                    map[hex / mapSize, hex % mapSize] = biome;
            }
        }

        public static void MakeMap() {
            do {
                GenerateMap();
                islands = DetectIslands();
            } while (!ValidateIslands(islands));
            SetIslandBiomes(islands);
        }

        public static int GetMapByteSize() => map.Length;
    }
}