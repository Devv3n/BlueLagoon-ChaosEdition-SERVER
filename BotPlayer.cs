namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    class Bot : Player {
        #region Const References
        static readonly Random random = new Random();
        static readonly string[] names = Resources.BotUsernames.Split("\r\n").Select(name => (name.Length > 28 ? name[..28] : name)).ToArray();
        #endregion

        #region The Artificial Intelligence
        public async void MakeMove() {
            // Prevent instant movement
            await Task.Delay(1000);

            // Obtain a list of possible places a bot could place a settler (needs to be land or have land nearby)/village at
            List<Hexagon> possibleSettlers = new List<Hexagon>();
            List<Hexagon> possibleSettlersResources = new List<Hexagon>();
            List<Hexagon> possibleVillages = new List<Hexagon>();
            foreach (Hexagon hex in GameHandler.map) {
                if (hex.settler != this && ((Program.gameStatus == 2 && hex.biome == 0) || hex.FindNearbySettler(this))) {
                    if (hex.biome != 0 || hex.FindNearbyLand()) {
                        possibleSettlers.Add(hex);

                        if (hex.resource != -1)
                            possibleSettlersResources.Add(hex);
                    }
                }

                else if (hex.settler == this && !hex.village && hex.biome != 0)
                    possibleVillages.Add(hex);
            }

            // Make a move
            while (true) {
                // Prioritie nearby resources
                if (possibleSettlersResources.Count > 0) {
                    Hexagon hex = possibleSettlersResources[0];
                    if (GameHandler.PlaceSettler(this, 0, hex.y, hex.x))
                        break;
                    else
                        possibleSettlersResources.Remove(hex);
                }

                // Prioritise placing villages if no villages placed && no nearby resources
                else if (villagesPlaced == 0 && villageCount != 0 && possibleVillages.Count != 0) {
                    Hexagon hex = possibleVillages[random.Next(possibleVillages.Count)];
                    if (GameHandler.PlaceSettler(this, 1, hex.y, hex.x))
                        break;
                    else
                        possibleVillages.Remove(hex);
                }

                // In the end, place a random settler/village somewhere
                else {
                    int choice = random.Next(possibleSettlers.Count + possibleVillages.Count);

                    // Place a settler randomly
                    if (choice < possibleSettlers.Count) {
                        Hexagon hex = possibleSettlers[choice];
                        if (GameHandler.PlaceSettler(this, 0, hex.y, hex.x))
                            break;
                        else
                            possibleSettlers.Remove(hex);
                    }

                    // Place a village randomly
                    else {
                        Hexagon hex = possibleVillages[choice - possibleSettlers.Count];
                        if (GameHandler.PlaceSettler(this, 1, hex.y, hex.x))
                            break;
                        else
                            possibleVillages.Remove(hex);
                    }
                }
            }
        }
        #endregion

        #region Creation/Disponsal handling
        public Bot() {
            // Get this bot a username
            username = "Bot " + names[random.Next(names.Length)];

            Logging.Log($"Bot added: {username}");
        }
        public override bool IsAlive() => alive;
        public override void Shutdown() {
            if (alive) {
                alive = false;
                NetworkHandler.RemovePlayer(this);

                Logging.Log($"Bot removed: {username}");
            }
         }
        #endregion

        #region Useless like me
        public override void SendData(NetworkType type, byte[] data) { }
        public override void SendStatistic(StatisticsType statisticType) { }
        public override void SendCounterUpdate(int type) { }
        #endregion
    }
}
