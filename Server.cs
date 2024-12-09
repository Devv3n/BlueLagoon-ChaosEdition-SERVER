namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public partial class Server : Form {

        public Server() {
            InitializeComponent(); // wat dis

            #region Configuration loading
            if (File.Exists("config.txt")) {
                string[] text = File.ReadAllLines("config.txt");

                if (text.Length == 2) {
                    if (!string.IsNullOrWhiteSpace(text[0]) && int.TryParse(text[0], out _))
                        ServerPortBox.Text = text[0].Length > 5 ? text[0][..5] : text[0];
                    if (!string.IsNullOrWhiteSpace(text[1]) && int.TryParse(text[1], out _))
                        MapSizeBox.Text = text[1].Length > 3 ? text[1][..3] : text[1];
                }
            }
            #endregion
        }

        #region Game Beginning/End Handling
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
            NetworkHandler.SendScores(true);
            GameHandler.ResetAllPlayers(true);

            Program.gameStatus = 1;
            Invoke(() => ServerButton.Text = "Start Game");
            Invoke(() => MapSizeBox.Enabled = true);
            NetworkHandler.AddWaitingClients();
        }
        #endregion

        #region Scaling UI
        private void Form_Resize(object sender, EventArgs e) {
            float scale = Math.Min(this.Size.Width / 640f, this.Size.Height / 360f) * 96f / (this.DeviceDpi * 1.05f);

            ServerButton.Font = new Font(ServerButton.Font.FontFamily, 18 * scale);
            ServerPortBox.Font = new Font(ServerPortBox.Font.FontFamily, 23f * scale);
            ServerPortBox.Top = (ServerPortBoxBackground.Height - ServerPortBox.Height) / 2;
        }
        #endregion

        #region Main button handling
        private void ServerButton_Click(object sender, EventArgs e) {
            switch (Program.gameStatus) {
                // Start server
                case 0: {
                        // Invalid port entered
                        if (!int.TryParse(ServerPortBox.Text, out int port) || 0 > port || port > 65565) {
                            MessageBox.Show($"Invalid port entered!", "bad port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logging.LogException($"Invalid port '{ServerPortBox.Text}'");
                        }

                        else {
                            // Start server
                            try {
                                Program.gameStatus = 1;
                                NetworkHandler.StartServer(port);

                                ServerButton.Text = "Start Game";
                                ServerPortBox.Enabled = false;

                                SaveConfig();
                                Logging.Log("Server started");
                            }
                            // Unhandled exception starting server??
                            catch (Exception ex) {
                                Program.gameStatus = 0;
                                MessageBox.Show(ex.Message, "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Logging.LogException(ex.Message);
                            }
                        }
                        break;
                    }

                // Start game
                case 1: {
                        // Check if sufficient player count (at least 2)
                        if (NetworkHandler.clients.Count >= 1) {
                            Program.gameStatus = 2;
                            ServerButton.Text = "...";
                            MapSizeBox.Enabled = false;

                            StartGame();
                            Logging.Log($"Game started with map size {GameHandler.mapSize}");
                        }

                        // Error if less than 2 players
                        else {
                            MessageBox.Show("Not enough players to start the game!", "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Logging.LogException($"Lack of players ({2 - NetworkHandler.clients.Count} more needed)");
                        }

                        break;
                    }
            }
        }
        #endregion

        #region Player List Handling
        public void AddPlayer(Client client) {
            // Setup label
            Label lbl = new Label();
            lbl.Text = client.username;
            lbl.Tag = client;

            // Interactive properties setup
            lbl.MouseEnter += PlayerText_MouseEnter;
            lbl.MouseLeave += PlayerText_MouseLeave;
            lbl.Click += PlayerText_Click;
            
            // Display label
            tableLayoutPanel3.Controls.Add(lbl);
         }

        private void PlayerText_Click(object? sender, EventArgs e) {
            if (sender is Label lbl && lbl.Tag is Client client)
                client.CloseClient();
        }

        private void PlayerText_MouseEnter(object? sender, EventArgs e) {
            if (sender is Label lbl) {
                lbl.ForeColor = Color.Red;
                lbl.Font = new Font(lbl.Font, FontStyle.Strikeout);
            }
        }

        private void PlayerText_MouseLeave(object? sender, EventArgs e) {
            if (sender is Label lbl) {
                lbl.ForeColor = Color.Black;
                lbl.Font = new Font(lbl.Font, FontStyle.Regular);
            }
        }
        #endregion

        void SaveConfig() {
            using (StreamWriter file = new StreamWriter("config.txt", false)) {
                file.WriteLine(ServerPortBox.Text);
                file.WriteLine(MapSizeBox.Text);
            }
        }

        #region Partial input sanitisation (ensure a number is entered)
        string previousPortBoxText = "";
        private void ServerPortBox_TextChanged(object sender, EventArgs e) {
            if (!int.TryParse(ServerPortBox.Text, out _))
                ServerPortBox.Text = previousPortBoxText;
            else
                previousPortBoxText = ServerPortBox.Text;
        }

        string previousMapSizeText = "";
        private void MapSizeBox_TextChanged(object sender, EventArgs e) {
            if (!int.TryParse(MapSizeBox.Text, out _))
                MapSizeBox.Text = previousMapSizeText;
            else
                previousMapSizeText = MapSizeBox.Text;

            SaveConfig();
        }
        #endregion

    }
}
