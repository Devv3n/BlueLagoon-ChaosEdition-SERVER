namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    class Bot : Player {
        #region Const References
        static readonly Random random = new Random();
        static readonly string[] names = Resources.BotUsernames.Split("\r\n");
        /*static readonly string[] names = File.Exists("BotUsernames.txt") ? 
            File.ReadAllLines("usernames.txt").Select(name => (name.Length > 32 ? name[..32] : name)).ToArray() // read username files
            : ["null"]; // no file :( */
        #endregion

        #region The Artificial Intelligence
        public async void MakeMove() {
            // Prevent instant movement
            await Task.Delay(1500);

            
        }
        #endregion

        #region Creation/Disponsal handling
        public Bot() {
            // Get this bot a username
            username = names[random.Next(names.Length)];

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
