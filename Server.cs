namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public partial class Server : Form {
        public int gameStatus = 0; //0-noServer 1-noMap 2/3-gamingStage

        public Server() {
            InitializeComponent(); // wat dis

            // Configuration loading
            if (File.Exists("config.txt")) {
                string[] text = File.ReadAllLines("config.txt");

                if (text.Length == 2) {
                    if (!string.IsNullOrWhiteSpace(text[0]) && int.TryParse(text[0], out _))
                        ServerPortBox.Text = text[0].Length > 5 ? text[0][..5] : text[0];
                    if (!string.IsNullOrWhiteSpace(text[1]) && int.TryParse(text[1], out _))
                        MapSizeBox.Text = text[1].Length > 3 ? text[1][..3] : text[1];
                }
            }
        }

        // Game beginning & end handlers
        public void StartGame() {
            // Input sanitisation to limit a number
            int mapSize = int.Parse(MapSizeBox.Text);
            if (mapSize < 10)
                mapSize = 10;
            else if (mapSize > 255)
                mapSize = 255;
            MapSizeBox.Text = mapSize.ToString();

            // Set up game
            GameHandler.MakeMap(mapSize);
            Client.defaultSettlerCount = 5 * mapSize / NetworkHandler.clients.Count;

            GameHandler.ResetAllPlayers(true);
            GameHandler.turn = NetworkHandler.clients[0];
            NetworkHandler.SendAllClients(222, [0]);
            NetworkHandler.SendAllClients(240, [3]); // Games played statistic

            NetworkHandler.SendMap();
        }
        public void FinishGame() {
            NetworkHandler.SendScores();
            GameHandler.ResetAllPlayers(true);

            gameStatus = 1;
            Invoke(() => ServerButton.Text = "Start Game");
            NetworkHandler.AddWaitingClients();
        }

        // Scaling UI
        private void Form_Resize(object sender, EventArgs e) {
            float scale = Math.Min(this.Size.Width / 640f, this.Size.Height / 360f) * 96f / (this.DeviceDpi * 1.05f);

            ServerButton.Font = new Font(ServerButton.Font.FontFamily, 18 * scale);
            ServerPortBox.Font = new Font(ServerPortBox.Font.FontFamily, 23f * scale);
            ServerPortBox.Top = (ServerPortBoxBackground.Height - ServerPortBox.Height) / 2;
        }

        // Main button handling
        private void ServerButton_Click(object sender, EventArgs e) {
            switch (gameStatus) {
                // Start server
                case 0: {
                        if (!int.TryParse(ServerPortBox.Text, out int port)) {
                            MessageBox.Show($"Invalid port entered!", "bad port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logging.LogException($"Invalid port '{ServerPortBox.Text}'");
                        }
                        else {
                            try {
                                gameStatus = 1;
                                NetworkHandler.StartServer(port);
                                ServerButton.Text = "Start Game";
                                ServerPortBox.Enabled = false;
                                SaveConfig();
                                Logging.Log("Server started");
                            }
                            catch (Exception ex) {
                                gameStatus = 0;
                                MessageBox.Show(ex.Message, "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Logging.LogException(ex.Message);
                            }
                        }
                        break;
                    }

                // Start game
                case 1: {
                        if (NetworkHandler.clients.Count >= 2) {
                            gameStatus = 2;
                            ServerButton.Text = "...";
                            StartGame();
                            Logging.Log($"Game started with map size {GameHandler.mapSize}");
                        }
                        else {
                            MessageBox.Show("Not enough players to start the game!", "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logging.LogException($"Lack of players ({2 - NetworkHandler.clients.Count} more needed)");
                        }

                        break;
                    }
            }
        }

        void SaveConfig() {
            using (StreamWriter file = new StreamWriter("config.txt", false)) {
                file.WriteLine(ServerPortBox.Text);
                file.WriteLine(MapSizeBox.Text);
            }
        }

        // Partial input sanitisation (ensure a number is entered)
        string previousText = "";
        private void MapSizeBox_TextChanged(object sender, EventArgs e) {
            if (!int.TryParse(MapSizeBox.Text, out _))
                MapSizeBox.Text = previousText;
            else
                previousText = MapSizeBox.Text;

            SaveConfig();
        }
    }
}
