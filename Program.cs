using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

// "2-Dimensional Representation Of A 3-Dimensional Cross-Section Of A 4-Dimensional Cube"
//      +___________+
//     /:\         ,:\
//    / : \       , : \
//   /  :  \     ,  :  \
//  /   :   +-----------+
// +....:../:...+   :  /|
// |\   +./.:...`...+ / |
// | \ ,`/  :   :` ,`/  |
// |  \ /`. :   : ` /`  |
// | , +-----------+  ` |
// |,  |   `+...:,.|...`+
// +...|...,'...+  |   /
//  \  |  ,     `  |  /
//   \ | ,       ` | /
//    \|,         `|/   mn, 7/97
//     +___________+

namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    internal static class Program {
        public static Server form = new Server();

        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //"yeah yeah no one cares"
            //- DevvEn

            GameHandler.noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            ApplicationConfiguration.Initialize();

            Application.Run(form);

            Logging.CloseLog();
        }
    }

    // file handling!!
    public static class Logging {
        static StreamWriter sw = new StreamWriter("Log.txt");

        public static void Log(string? message) {
            sw?.WriteLine($"<{CurrentTime()}> {message}");
        }

        public static void LogException(string? message) {
            sw?.WriteLine($"[{CurrentTime()}] {message}");
        }

        public static void CloseLog() {
            sw?.WriteLine($"|{CurrentTime()}| Goodbye!");
            sw?.Close();
        }

        static string CurrentTime() => DateTime.Now.ToString("HH:mm:ss");
    }


    public class Client {
        public bool alive = true;
        public TcpClient client;
        public NetworkStream stream;
        public string username;
        public Color color;

        public int[] resourceCount = new int[6];
        public int settlerCount = 30;
        public int villageCount = 3;

        public Client(TcpClient client) {
            this.client = client;
            stream = client.GetStream();

            byte[] buffer = new byte[128];
            stream.Read(buffer, 0, buffer.Length);
            username = Encoding.Unicode.GetString(buffer);

            Random rng = new Random();
            color = Color.FromArgb(255, rng.Next(256), rng.Next(256), rng.Next(256));

            Task.Run(HandleData);
            Logging.Log($"New client '{username}'");
        }

        // Attempts to send a length of 0 byte array and if no errors then client probablyyy still connected
        public bool IsAlive() {
            if (alive) {
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

                Logging.Log($"Client '{username}' left");
            }
        }
        
        // Data handling (sending/reading) functions
        public void SendData(byte type, byte[]? data) {
            if (alive) {
                try {
                    stream.WriteByte(type);
                    if (data != null)
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
    }

    public static class NetworkHandler {
        public static List<Client> clients = new List<Client>();
        public static List<Client> deadClients = new List<Client>();
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
                foreach (Client c in clients) {
                    if (c != client) {
                        client.SendData(220, Encoding.Unicode.GetBytes(c.username));
                    }
                    c.SendData(220, Encoding.Unicode.GetBytes(client.username));
                }

                Label lbl = new Label();
                lbl.Text = client.username;
                Program.form.tableLayoutPanel3.Controls.Add(lbl);
            }
        }

        // Dead client handling
        public static void RemoveClient(Client client) {
            client.CloseClient();
            if (Program.form.gameStatus == 1)
                clients.Remove(client);
            else
                deadClients.Add(client);
        }
        public static void RemoveDeadClients() {
            foreach (Client deadClient in deadClients)
                RemoveClient(deadClient);
            deadClients.Clear();
        }

        // Sending data to clients functions
        public static void SendAllClients(byte type) {
            foreach (Client client in clients)
                client.SendData(type, null);
        }
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
    }



    public class Hexagon(int biome, int y, int x) {
        // Const references
        public static readonly int[][] hexOffsets0 = [[-1, -1], [-1, 0], [0, 1], [1, 0], [1, -1], [0, -1]];
        public static readonly int[][] hexOffsets1 = [[-1, 0], [-1, 1], [0, 1], [1, 1], [1, 0], [0, -1]];
        static readonly int[][] biomeResourceTypes = [[0, 1, 2, 3, 4, 5], [0, 1, 2, 3, 4], [0, 2, 3]];
        static readonly Random random = new Random();

        // Hex configuration
        public int y = y;
        public int x = x;
        public int biome = biome;
        public int resource = -1;
        public Client? settler;
        public bool village = false;

        // Hex configuring functions
        public void SetRandomResource() {
            int[] resourceTypes = biomeResourceTypes[biome - 1];
            while (true) {
                resource = resourceTypes[random.Next(resourceTypes.Length)];
                switch (resource) {
                        case 3:    
                        case 5: {
                            if (!FindNearbyWater())
                                continue;
                            break;
                        }

                        case 4: {
                            if (FindNearbyWater())
                                continue;
                            break;
                        }
                }

                break;
            }
        }

        // Search neighbouring hexes functions
        static bool WithinMapBounds(int y, int x) => 0 <= y && y < GameHandler.mapSize && 0 <= x && x < GameHandler.mapSize;
        public bool FindNearbyResources() {
            foreach (int[] offset in y % 2 == 0 ? hexOffsets0 : hexOffsets1) 
                if (WithinMapBounds(y + offset[0], x + offset[1]) && GameHandler.map[y + offset[0], x + offset[1]].resource != -1)
                    return true;

            return false;
        }
        public bool FindNearbyWater() {
            foreach (int[] offset in y % 2 == 0 ? hexOffsets0 : hexOffsets1)
                if (WithinMapBounds(y + offset[0], x + offset[1]) && GameHandler.map[y + offset[0], x + offset[1]].biome == 0)
                    return true;

            return false;
        }
        public bool FindNearbySettler(Client settler) {
            foreach (int[] offset in y % 2 == 0 ? hexOffsets0 : hexOffsets1)
                if (WithinMapBounds(y + offset[0], x + offset[1]) && GameHandler.map[y + offset[0], x + offset[1]].settler == settler)
                    return true;

            return false;
        }
    }

    public static class GameHandler {
        // Const referneces
        public static readonly FastNoiseLite noise = new FastNoiseLite();
        static readonly Random random = new Random();

        // Map configuration
        public static int mapSize;
        public static Hexagon[,] map;
        static List<List<int>>? islands;
    
        // Game Configuration
        public static Client? turn;
        static int? resourceCount;

        // Functions for generating map
        static void GenerateMap() {
            int xOffset = random.Next(-10000, 10000);
            int yOffset = random.Next(-10000, 10000);

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    map[y, x] = new Hexagon(noise.GetNoise(x * 15 + xOffset, y * 15 + yOffset) > 0.05 ? 1 : 0, y, x);
                }
            }
        }
        static List<List<int>> DetectIslands() {
            bool[,] searched = new bool[mapSize, mapSize];
            islands = new List<List<int>>();

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (searched[y, x] || map[y, x].biome == 0) {
                        searched[y, x] = true;
                        continue;
                    }

                    List<int> islandPos = new List<int>();

                    void SearchNearby(int y, int x) {
                        if (0 <= y && y < mapSize && 0 <= x && x < mapSize && !searched[y, x]) {
                            searched[y, x] = true;
                            if (map[y, x].biome == 0)
                                return;

                            islandPos.Add(y * mapSize + x);
                            foreach (int[] o in (y % 2 == 0 ? Hexagon.hexOffsets0 : Hexagon.hexOffsets1))
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
                if (island.Count > (mapSize/3))
                    bigIslandCount++;
                else {
                    foreach (int hex in island)
                        map[hex / mapSize, hex % mapSize].biome = 0;
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
                    map[hex / mapSize, hex % mapSize].biome = biome;
            }
        }
        static int[] GenerateResources() {
            resourceCount = 0;
            List<int> resourcePos = new List<int>();
            
            int attempts = mapSize * mapSize;
            while (resourceCount < mapSize*3/2 && attempts-- > 0) {
                Hexagon hex = map[random.Next(mapSize), random.Next(mapSize)];

                if (hex.biome != 0 && hex.resource == -1 && !hex.FindNearbyResources()) {
                    hex.SetRandomResource();
                    
                    resourceCount++;
                    resourcePos.Add(hex.y * mapSize + hex.x);
                }
            }
            
            Logging.Log($"Generated {resourceCount}/{mapSize * 3 / 2} resources");
            return resourcePos.ToArray();
        }
        public static void MakeMap(int size) {
            mapSize = size;
            map = new Hexagon[mapSize, mapSize];
            do {
                GenerateMap();
                islands = DetectIslands();
            } while (!ValidateIslands(islands));
            SetIslandBiomes(islands);
            GenerateResources();
        }
    
        // Gameplay functions
        static bool CheckForEnd() {
            if (resourceCount == 0)
                return true;

            foreach (Client client in NetworkHandler.clients) {
                if (client.settlerCount != 0)
                    return false;
            }

            return true;
        }
        static void NextRound() {
            if (Program.form.gameStatus == 2) {
                Program.form.gameStatus = 3;
                turn = NetworkHandler.clients[0];

                foreach (Hexagon hex in map) {
                    hex.resource = -1;
                    if (!hex.village) {
                        hex.settler = null;
                        hex.village = false;
                    }
                }

                NetworkHandler.SendAllClients(212);
                int[] resourcePos = GenerateResources();
                foreach (int pos in resourcePos) {
                    NetworkHandler.SendHexUpdate(map[pos / mapSize, pos % mapSize], -1);
                }
            }
            else if (Program.form.gameStatus == 3)
                Program.form.FinishGame();
        }
        public static void ChooseNextPlayer() {
            if (CheckForEnd())
                NextRound();

            else {
                int index = NetworkHandler.clients.IndexOf(turn);
                int loops = 0;

                // Find next alive client
                while (loops <= 1) {
                    index = ++index % NetworkHandler.clients.Count;
                    if (index == 0)
                        loops++;

                    Client next = NetworkHandler.clients[index];
                    if (next.IsAlive() && next != turn) {
                        turn = next;
                        break;
                    }
                }

                // If the while loop did more than 1 loop around the client list then it means everyone is dead
                if (loops > 1)
                    Program.form.FinishGame();
            }
        }

        // Update hex to place settler/village & send everyone hex update
        public static void PlaceSettler(Client client, int type, int y, int x) {
            if (turn == client && 0 <= x && x < mapSize && 0 <= y && y < mapSize) {
                Hexagon hex = map[y, x];

                int settlerUsage = hex.settler == null ? 1 : 3;
                if (type == 0 && hex.settler != client && !hex.village && client.settlerCount >= settlerUsage) {
                    if ((Program.form.gameStatus == 2 && hex.biome == 0) || hex.FindNearbySettler(client)) {
                        client.settlerCount -= settlerUsage;
                        hex.settler = client;
                        
                        if (hex.resource != -1) {
                            client.resourceCount[hex.resource]++;
                            hex.resource = -1;
                        }
                        
                        NetworkHandler.SendHexUpdate(hex, 0);
                        ChooseNextPlayer();
                    }
                }

                else if (type == 1 && hex.settler == client && !hex.village && client.villageCount > 0) {
                    client.villageCount--;
                    hex.village = true;

                    NetworkHandler.SendHexUpdate(hex, 1);
                    ChooseNextPlayer();
                }
            }
        }
    }
}