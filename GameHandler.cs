namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
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

        // Gameplay (settler) variables
        public Player? settler;
        public bool village = false;
        #endregion

        #region Gameplay Functions
        // Generate a resource until all conditions met
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

        // Remove any village/settler at hexagon used for natural disasters
        public void Clear() {
            if (settler != null && village) {
                settler.villagesPlaced--;
                village = false;
            }
            settler = null;

            NetworkHandler.SendAllPlayers(NetworkType.HexClear, [(byte)y, (byte)x]);
        }
        #endregion

        #region Search Neighbouring Hexes Functions
        public static bool WithinMapBounds(int y, int x) => 0 <= y && y < GameHandler.mapSize && 0 <= x && x < GameHandler.mapSize;
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
        public bool FindNearbyLand() {
            foreach (int[] offset in y % 2 == 0 ? hexOffsets0 : hexOffsets1)
                if (WithinMapBounds(y + offset[0], x + offset[1]) && GameHandler.map[y + offset[0], x + offset[1]].biome != 0)
                    return true;

            return false;
        }
        public bool FindNearbySettler(Player settler) {
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
        public static Player? turn;
        static int? resourceCount;
        static int? resourceTypes;
        #endregion

        #region Map Generation Functions
        static Hexagon[,] GenerateMap(Hexagon[,] map) {
            // Offset to get random map every time
            int xOffset = random.Next(-10000, 10000);
            int yOffset = random.Next(-10000, 10000);
            float scale = 300f / mapSize;

            // Height map generation - whether location should have land
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    map[y, x] = new Hexagon(noise.GetNoise(x * scale + xOffset, y * scale + yOffset) > 0.05 ? 1 : 0, y, x);
                }
            }

            return map;
        }
        static List<List<Hexagon>> DetectIslands(Hexagon[,] map) {
            //  Variables
            bool[,] searched = new bool[mapSize, mapSize];
            List<List<Hexagon>> _islands = new List<List<Hexagon>>();

            // Loop through every hexagon
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    // Ignore hex if searched || has no land
                    if (searched[y, x] || map[y, x].biome == 0) {
                        searched[y, x] = true;
                        continue;
                    }

                    // Find hexagons on this island
                    List<Hexagon> hexes = new List<Hexagon>();
                    void SearchNearby(int y, int x) { // don't ask me how this works, only god remembers
                        if (Hexagon.WithinMapBounds(y,x) && !searched[y, x]) {
                            searched[y, x] = true;
                            if (map[y, x].biome == 0)
                                return;

                            hexes.Add(map[y, x]);
                            foreach (int[] o in (y % 2 == 0 ? Hexagon.hexOffsets0 : Hexagon.hexOffsets1))
                                SearchNearby(y + o[0], x + o[1]);
                        }
                    }
                    SearchNearby(y, x);

                    // Add to list if island sufficiently sized
                    if (hexes.Count > (mapSize / 3))
                        _islands.Add(hexes);

                    // Completely wipe any traces of the island from existence if too small
                    else
                        foreach (Hexagon hex in hexes)
                            hex.biome = 0;
                }
            }

            return _islands;
        }
        static bool ValidateIslands(List<List<Hexagon>> islands) { //obsolete ig used to be useful in early versions but i dislike removing stuff
            // Check if sufficient island count
            return islands.Count == 8;
        }
        static void SetIslandBiomes() { // Also assigns island variable to hexagons
            int index = 0; // Used for determining which island hex is part of

            foreach (List<Hexagon> island in islands) {
                // Set biome of an island
                int biome = random.Next(1, 4);

                // Assign island's biome to each hex within
                foreach (Hexagon hex in island) {
                    hex.biome = biome;
                    hex.island = index;
                }

                index++;
            }
        }
        static Hexagon[] GenerateResources() {
            // Resource Variables
            resourceCount = 0;
            bool[] uniqueResources = new bool[7];
            List<Hexagon> resources = new List<Hexagon>();

            // Limited attempts to prevent an infinite loop
            int attempts = mapSize * mapSize;
            while (resourceCount < mapSize * 3 / 2 && attempts-- > 0) {
                Hexagon hex = map[random.Next(mapSize), random.Next(mapSize)];

                if (hex.biome != 0 && hex.resource == -1 && !hex.FindNearbyResources() && !hex.village) {
                    // Generate resource at hex && add to list
                    hex.SetRandomResource();
                    resources.Add(hex);

                    // Unique resource
                    if (hex.resource != 0)
                        uniqueResources[hex.resource - 1] = true;

                    // Resource count
                    resourceCount++;
                }
            }

            // Find how many unique resources there are
            resourceTypes = 0;
            foreach (bool resourceType in uniqueResources)
                if (resourceType)
                    resourceTypes++;

            Logging.Log($"Generated {resourceCount}/{mapSize * 3 / 2} resources");
            return resources.ToArray();
        }

        // Main map generation function - multithreaded!!
        public static void MakeMap(int size) {
            mapSize = size;

            Parallel.For(0, Environment.ProcessorCount, (int _, ParallelLoopState state) => {
                Hexagon[,] _map = new Hexagon[mapSize, mapSize];

                while (!state.IsStopped) {
                    // Generate "things"
                    _map = GenerateMap(_map);
                    List<List<Hexagon>> _islands = DetectIslands(_map);

                    // Validate & send to main program
                    if (ValidateIslands(_islands) && !state.IsStopped) {
                        state.Stop();
                        map = _map;
                        islands = _islands;
                    }
                }
            });

            SetIslandBiomes();
            GenerateResources();
        }
        #endregion

        #region Gameplay Functions
        public static void ChooseNextPlayer() {
            // First check if is the end of the round
            if (CheckForEnd())
                NextRound();

            else {
                int index = NetworkHandler.players.IndexOf(turn);
                int loops = 0;

                // Find next alive player
                while (loops <= 1) {
                    if (NetworkHandler.players.Count == 0) {
                        loops = 2;
                        break;
                    }
                    index = ++index % NetworkHandler.players.Count;
                    
                    // Loop count calculator
                    if (index == 0)
                        loops++;

                    // Next player chooser
                    Player next = NetworkHandler.players[index];
                    if (next.IsAlive() && next != turn && (Program.gameStatus == 2 || (Program.gameStatus == 3 && next.villagesPlaced != 0))) {
                        turn = next;
                        NetworkHandler.SendAllPlayers(NetworkType.PlayerTurn, [(byte)index]);

                        // 1/playerCount% chance of natural disaster each turn (1% each round)
                        if (random.Next(100 * NetworkHandler.players.Count) == 48)
                            NaturalDisater();

                        // Makes bots actually do something
                        if (turn is Bot bot)
                            Task.Run(bot.MakeMove);

                        break;
                    }
                }

                // If the while loop did more than 1 loop around the client list then it means everyone is dead
                if (loops > 1)
                    Program.form.FinishGame(false);
            }
        }

        public static void NaturalDisater() {
            int type = random.Next(30);
            
            // 3.3% Revolt (a village is burned)
            if (type == 29 && Program.gameStatus != 3) {
                // Obtain a list of villages
                List<Hexagon> villageList = new List<Hexagon>();
                foreach (Hexagon hex in map)
                    if (hex.village)
                        villageList.Add(hex);

                // Destroy a random village & alert people of this "natural disaster"
                if (villageList.Count > 0) {
                    NetworkHandler.SendDisaster(Disaster.Revolt);
                    villageList[random.Next(villageList.Count)].Clear();
                    return; // prevent any other conditions being checked
                } 
            }

            // 30% Mysterious deadly force on an island clearing settlers
            if (type > 20) {
                NetworkHandler.SendDisaster(Disaster.MysteriousDeadlyForce);
                foreach (Hexagon hex in map)
                    if (hex.settler != null && !hex.village && random.Next(mapSize / 2) == 0)
                            hex.Clear();
            }

            // 66.7% Map wide flood/tornado clearing settlers
            else {
                NetworkHandler.SendDisaster(random.Next(2) == 1 ? Disaster.Flood : Disaster.Tornado);
                foreach (Hexagon hex in map)
                    if (hex.settler != null && !hex.village && random.Next(mapSize * 5) == 0)
                        hex.Clear();
            }
        }

        public static bool PlaceSettler(Player player, int type, int y, int x) {
            // Check if player's turn and validate hexagon position
            if (turn == player && Hexagon.WithinMapBounds(y, x)) {
                Hexagon hex = map[y, x];

                // Settler placement
                int settlerUsage = hex.settler == null ? 1 : 3;
                if (type == 0 && hex.settler != player && !hex.village && player.settlerCount >= settlerUsage) {
                    if ((Program.gameStatus == 2 && hex.biome == 0) || hex.FindNearbySettler(player)) {
                        // Take 1/3 settler(s) from client and place
                        player.settlerCount -= settlerUsage;
                        hex.settler = player;

                        // Give client resource (if any) at hex
                        if (hex.resource != -1) {
                            player.resourceCount[hex.resource]++;
                            hex.resource = -1;
                        }

                        // Send map/statistics update networking
                        player.SendStatistic(StatisticsType.SettlersPlaced);
                        player.SendCounterUpdate(0);
                        NetworkHandler.SendHexUpdate(hex, 0);

                        ChooseNextPlayer();
                        return true;
                    }
                }

                // Village placement
                else if (Program.gameStatus == 2 && type == 1 && hex.biome != 0 && hex.settler == player && !hex.village && player.villageCount > 0) {
                    // Take village from client and place
                    player.villageCount--;
                    player.villagesPlaced++;
                    hex.village = true;

                    // Send map/statistics update networking
                    player.SendStatistic(StatisticsType.VillagesPlaced);
                    player.SendCounterUpdate(1);
                    NetworkHandler.SendHexUpdate(hex, 1);

                    ChooseNextPlayer();
                    return true;
                }
            }

            // If no valid placement then return false
            return false;
        }
        #endregion

        #region End Functions
        static void NextRound() {
            if (Program.gameStatus == 2) {
                Program.gameStatus = 3;
                NetworkHandler.SendScores(false);

                // Reset to default variables
                ResetAllPlayers(false);
                turn = NetworkHandler.players[0];
                if (turn.villageCount == 0)
                    ChooseNextPlayer();
                else
                    NetworkHandler.SendAllPlayers(NetworkType.PlayerTurn, [0]);

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
                List<Player> villageless = new List<Player>(NetworkHandler.players);
                foreach (Hexagon hex in villages) {
                    NetworkHandler.SendHexUpdate(hex, hex.village ? 1 : 0);
                    villageless.Remove(hex.settler);
                }

                // Statistic for people with no villages during settlement phase
                foreach (Player player in villageless)
                    player.SendStatistic(StatisticsType.SettlementPhasesUnplayable);

                // Leaderboard delay
                Thread.Sleep(5000);

                // if it is a silly bot as first player make them do something
                if (turn is Bot bot)
                    bot.MakeMove();
            }
            else if (Program.gameStatus == 3)
                Program.form.FinishGame(true);
        }
        static bool CheckForEnd() {
            // No resources on map left
            if (resourceCount == 0)
                return true;

            // A player has ran out of settlers
            foreach (Player player in NetworkHandler.players) {
                if (player.settlerCount == 0)
                    return true;
            }

            return false;
        }
        public static void ResetAllPlayers(bool fullReset) {
            foreach (Player player in NetworkHandler.players) {
                player.Reset(fullReset);
            }
        }

        public static void CalculatePlayerScores() {
            // Reset players' temp score variables + calculate score from resources
            foreach (Player player in NetworkHandler.players) {
                // Reset values
                player._score = 0;
                player._islandSettlerCount = 0;
                player._uniqueIslands = new bool[8];
                player._linkedIslands = 0;

                // Resource score
                int uniqueResources = 0;
                foreach (int count in player.resourceCount.Skip(1)) {
                    if (count > 0)
                        uniqueResources++;

                    if (count >= 2)
                        player._score += 60 * (int)Math.Pow(2, count - 1) / mapSize;
                }

                // Statuette score
                player._score += 65 / mapSize * player.resourceCount[0];

                // Score if player has all available resource types
                if (uniqueResources == resourceTypes)
                    player._score += 10;
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
                Player dominant = NetworkHandler.players[0];
                bool draw = false;
                foreach (Player client in NetworkHandler.players) {
                    dominant = client._islandSettlerCount > dominant._islandSettlerCount ? client : dominant;
                    draw = client._islandSettlerCount == dominant._islandSettlerCount;
                }

                // Add score to dominant player
                if (!draw)
                    dominant._score += island.Count * 20 / mapSize;

                // Reset settler count on island for each player
                foreach (Player player in NetworkHandler.players)
                    player._islandSettlerCount = 0;

                islandIndex++;
            }

            // Unique islands settled on calculation
            foreach (Player player in NetworkHandler.players) {
                // Count amount of islands client has been on
                int uniqueIslands = 0;
                foreach (bool island in player._uniqueIslands)
                    if (island)
                        uniqueIslands++;

                // Add score if sufficient unique islands
                if (uniqueIslands == 8)
                    player._score += 20;
                else if (uniqueIslands == 7)
                    player._score += 10;
            }

            // Islands linked calculation
            bool[,] searched = new bool[mapSize, mapSize];
            for (int y = 0; y < mapSize; y++) {
                for (int x = 0; x < mapSize; x++) {
                    // Ignore searched hexes
                    if (searched[y, x])
                        continue;

                    // Find a chain of hexes
                    Player? searchSettler = map[y, x].settler;
                    void SearchNearby(int y, int x) {
                        if (Hexagon.WithinMapBounds(y, x)) {
                            Hexagon hex = map[y, x];
                            Player? settler = hex.settler;

                            // No settler at hex
                            if (settler == null)
                                searched[y, x] = true;

                            // Client's settler present
                            else if (!searched[y, x] && settler == searchSettler) {
                                searched[y, x] = true;
                                settler._uniqueIslands[hex.island] = true;

                                foreach (int[] o in (y % 2 == 0 ? Hexagon.hexOffsets0 : Hexagon.hexOffsets1))
                                    SearchNearby(y + o[0], x + o[1]);
                            }
                        }
                    }

                    if (searchSettler != null) {
                        // Initiate search
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
            foreach (Player player in NetworkHandler.players) { // Scoring
                if (player._linkedIslands >= 2)
                    player._score += player._linkedIslands * 5;
            }
        }
        #endregion
    }
}
