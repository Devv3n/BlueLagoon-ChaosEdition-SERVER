﻿namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public class Hexagon(int biome, int y, int x) {
        #region Variables
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
        public int island;

        public Client? settler;
        public bool village = false;
        #endregion

        #region Hex Random Resource Generation
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
        #endregion

        #region Search Neighbouring Hexes Functions
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
        #endregion
    }

    public static class GameHandler {
        #region Variables
        // Const referneces
        public static readonly FastNoiseLite noise = new FastNoiseLite();
        static readonly Random random = new Random();

        // Map configuration
        public static int mapSize;
        public static Hexagon[,] map;
        static List<List<Hexagon>> islands;

        // Game Configuration
        public static Client? turn;
        static int? resourceCount;
        static int? resourceTypes;
        #endregion

        #region Map Generation Functions
        static void GenerateMap() {
            int xOffset = random.Next(-10000, 10000);
            int yOffset = random.Next(-10000, 10000);

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    map[y, x] = new Hexagon(noise.GetNoise(x * 15 + xOffset, y * 15 + yOffset) > 0.05 ? 1 : 0, y, x);
                }
            }
        }
        static void DetectIslands() {
            bool[,] searched = new bool[mapSize, mapSize];
            islands = new List<List<Hexagon>>();

            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (searched[y, x] || map[y, x].biome == 0) {
                        searched[y, x] = true;
                        continue;
                    }

                    List<Hexagon> hexes = new List<Hexagon>();

                    void SearchNearby(int y, int x) {
                        if (0 <= y && y < mapSize && 0 <= x && x < mapSize && !searched[y, x]) {
                            searched[y, x] = true;
                            if (map[y, x].biome == 0)
                                return;

                            hexes.Add(map[y, x]);
                            foreach (int[] o in (y % 2 == 0 ? Hexagon.hexOffsets0 : Hexagon.hexOffsets1))
                                SearchNearby(y + o[0], x + o[1]);
                        }
                    }

                    SearchNearby(y, x);
                    islands.Add(hexes);

                }
            }
        }
        static bool ValidateIslands() {
            int bigIslandCount = 0;
            List<List<Hexagon>> removeIslands = new List<List<Hexagon>>();

            foreach (List<Hexagon> island in islands) {
                if (island.Count > (mapSize / 3))
                    bigIslandCount++;
                else {
                    foreach (Hexagon hex in island)
                        hex.biome = 0;
                    removeIslands.Add(island);
                }
            }

            foreach (List<Hexagon> island in removeIslands)
                islands.Remove(island);

            if (bigIslandCount == 8 && islands.Count == 8)
                return true;
            return false;
        }
        static void SetIslandBiomes() { // Also assigns island variable to hexagons
            int index = 0;

            foreach (List<Hexagon> island in islands) {
                int biome = random.Next(1, 4);

                foreach (Hexagon hex in island) {
                    hex.biome = biome;
                    hex.island = index;
                }

                index++;
            }
        }
        static Hexagon[] GenerateResources() {
            resourceCount = 0;
            bool[] uniqueResources = new bool[7];
            List<Hexagon> resources = new List<Hexagon>();

            int attempts = mapSize * mapSize;
            while (resourceCount < mapSize * 3 / 2 && attempts-- > 0) {
                Hexagon hex = map[random.Next(mapSize), random.Next(mapSize)];

                if (hex.biome != 0 && hex.resource == -1 && !hex.FindNearbyResources() && !hex.village) {
                    hex.SetRandomResource();

                    if (hex.resource != 0)
                        uniqueResources[hex.resource - 1] = true;

                    resourceCount++;
                    resources.Add(hex);
                }
            }

            resourceTypes = 0;
            foreach (bool resourceType in uniqueResources)
                if (resourceType)
                    resourceTypes++;


            Logging.Log($"Generated {resourceCount}/{mapSize * 3 / 2} resources");
            return resources.ToArray();
        }
        public static void MakeMap(int size) {
            mapSize = size;
            map = new Hexagon[mapSize, mapSize];
            do {
                GenerateMap();
                DetectIslands();
            } while (!ValidateIslands());
            SetIslandBiomes();
            GenerateResources();
        }
        #endregion

        #region Gameplay Functions
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
                    if (next.IsAlive() && next != turn && (Program.form.gameStatus == 2 || (Program.form.gameStatus == 3 && next.villagePlaced))) {
                        turn = next;
                        NetworkHandler.SendAllClients(222, [(byte)index]);
                        break;
                    }
                }

                // If the while loop did more than 1 loop around the client list then it means everyone is dead
                if (loops > 1)
                    Program.form.FinishGame();
            }
        }
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

                        client.SendData(240, [0]);
                        NetworkHandler.SendHexUpdate(hex, 0); // Settlers placed statistics
                        ChooseNextPlayer();
                    }
                }

                else if (Program.form.gameStatus == 2 && type == 1 && hex.settler == client && !hex.village && client.villageCount > 0) {
                    client.villageCount--;
                    hex.village = true;

                    client.SendData(240, [1]);
                    NetworkHandler.SendHexUpdate(hex, 1); // Villages placed statistics
                    ChooseNextPlayer();
                }
            }
        }
        #endregion

        #region End Functions
        static async void NextRound() {
            if (Program.form.gameStatus == 2) {
                NetworkHandler.SendScores();

                // Reset to default
                ResetAllPlayers(false);
                turn = NetworkHandler.clients[0];
                NetworkHandler.SendAllClients(222, [0]);

                // Find villages & clear map where no villages
                List<Hexagon> villages = new List<Hexagon>();
                foreach (Hexagon hex in map) {
                    hex.resource = -1;
                    if (hex.village)
                        villages.Add(hex);
                    else
                        hex.settler = null;
                }

                // Setup & send resources again for settlement phase
                Hexagon[] resourcePos = GenerateResources();
                foreach (Hexagon hex in resourcePos)
                    NetworkHandler.SendHexUpdate(hex, -1);

                // Send village locations
                List<Client> villageless = new List<Client>(NetworkHandler.clients);
                foreach (Hexagon hex in villages) {
                    NetworkHandler.SendHexUpdate(hex, hex.village ? 1 : 0);
                    villageless.Remove(hex.settler);
                }

                // Statistic for people with no villages during settlement phase
                foreach (Client client in villageless)
                    client.SendData(240, [8]);

                // Leaderboard delay
                await Task.Delay(5000);
                Program.form.gameStatus = 3;
            }
            else if (Program.form.gameStatus == 3)
                Program.form.FinishGame();
        }
        static bool CheckForEnd() {
            if (resourceCount == 0)
                return true;

            foreach (Client client in NetworkHandler.clients) {
                if (client.settlerCount == 0)
                    return true;
            }

            return false;
        }
        public static void ResetAllPlayers(bool full) {
            foreach (Client client in NetworkHandler.clients) {
                client.Reset();
                if (full)
                    client.villagePlaced = false;
            }
        }

        public static int[] GetPlayerScores() {
            // Reset clients' temp score variables + calculate score from resources
            foreach (Client client in NetworkHandler.clients) {
                // Reset values
                client._score = 0;
                client._islandSettlerCount = 0;
                client._uniqueIslands = new bool[8];
                client._linkedIslands = 0;

                // Resource score
                int uniqueResources = 0;
                foreach (int count in client.resourceCount.Skip(1)) {
                    if (count > 0)
                        uniqueResources++;

                    if (count >= 2)
                        client._score += 60 * (int)Math.Pow(2, count - 1) / mapSize;
                }

                // Statuette score
                client._score += 65 / mapSize * client.resourceCount[0];

                // Score if player has all available resource types
                if (uniqueResources == resourceTypes)
                    client._score += 10;
            }

            // Island domination calculation
            int islandIndex = 0;
            foreach (List<Hexagon> island in islands) {
                // Count amount of settlers each player has on the island
                foreach (Hexagon hex in island) {
                    if (hex.settler != null) {
                        hex.settler._uniqueIslands[islandIndex] = true; // declare they have a settler on this island (used in next calculation)
                        hex.settler._islandSettlerCount++;
                    }
                }

                // Find person with most settlers on the island
                Client dominant = NetworkHandler.clients[0];
                bool draw = false;
                foreach (Client client in NetworkHandler.clients) {
                    dominant = client._islandSettlerCount > dominant._islandSettlerCount ? client : dominant;
                    draw = client._islandSettlerCount == dominant._islandSettlerCount;
                }

                // Add score to dominant player
                if (!draw)
                    dominant._score += island.Count * 20 / mapSize;

                // Reset settler count on island for each player
                foreach (Client client in NetworkHandler.clients)
                    client._islandSettlerCount = 0;

                islandIndex++;
            }

            // Unique islands settled on calculation
            foreach (Client client in NetworkHandler.clients) {
                // Count amount of islands client has been on
                int uniqueIslands = 0;
                foreach (bool island in client._uniqueIslands)
                    if (island)
                        uniqueIslands++;

                // Add score if sufficient unique islands
                if (uniqueIslands == 8)
                    client._score += 20;
                else if (uniqueIslands == 7)
                    client._score += 10;
            }

            // Islands linked calculation
            bool[,] searched = new bool[mapSize, mapSize];
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    if (searched[y, x])
                        continue;

                    Client? searchSettler = map[y, x].settler;
                    void SearchNearby(int y, int x) {
                        if (0 <= y && y < mapSize && 0 <= x && x < mapSize) {
                            Hexagon hex = map[y, x];
                            Client? settler = hex.settler;
                            bool searchingFor = settler == searchSettler;

                            if (settler == null)
                                searched[y, x] = true;
                            else if (!searched[y, x] && searchingFor) {
                                searched[y, x] = true;
                                settler._uniqueIslands[hex.island] = true;
                                foreach (int[] o in (y % 2 == 0 ? Hexagon.hexOffsets0 : Hexagon.hexOffsets1))
                                    SearchNearby(y + o[0], x + o[1]);
                            }
                        }
                    }

                    if (searchSettler != null) {
                        searchSettler._uniqueIslands = new bool[8];
                        SearchNearby(y, x);

                        // Count number of linked islands
                        int linkedIslands = 0;
                        foreach (bool island in searchSettler._uniqueIslands) {
                            if (island)
                                linkedIslands++;
                        }

                        // Determine whether there were more islands linked
                        if (linkedIslands > searchSettler._linkedIslands)
                            searchSettler._linkedIslands = linkedIslands;
                    }
                    else
                        searched[y, x] = true;
                }
            }
            foreach (Client client in NetworkHandler.clients) { // Scoring
                if (client._linkedIslands >= 2)
                    client._score += client._linkedIslands * 5;
            }


            // Put data into a byte[] to send over network
            return NetworkHandler.clients.Select(client => client._score).ToArray();

        }
        #endregion
    }
}
