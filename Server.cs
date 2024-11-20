namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    public partial class Server : System.Windows.Forms.Form {
        int gameStatus; //0-noServer 1-noMap 2-gaming
        
        public Server() {
            InitializeComponent(); // wat dis
            gameStatus = 0;
        }

        private void Form_Resize(object sender, EventArgs e) {
            float scale = Math.Min(this.Size.Width / 640f, this.Size.Height / 360f);

            ServerButton.Font = new Font(ServerButton.Font.FontFamily, 18 * scale);
            ServerPortBox.Font = new Font(ServerPortBox.Font.FontFamily, 23f * scale);
            ServerPortBox.Top = (ServerPortBoxBackground.Height - ServerPortBox.Height) / 2;
        }

        private void ServerButton_Click(object sender, EventArgs e) {
            switch (gameStatus) {
                case 0: {
                        if (!int.TryParse(ServerPortBox.Text, out int port))
                            MessageBox.Show($"Invalid port entered!", "bad port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else {
                            try {
                                NetworkHandler.StartServer(port);
                                gameStatus = 1;
                                ServerPortBox.Enabled = false;
                                ServerButton.Text = "Start Game";
                            }
                            catch (Exception ex) {
                                MessageBox.Show(ex.Message, "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;
                }

                case 1: {
                        if (NetworkHandler.clients.Count >= 2) {
                            gameStatus = 2;
                            ServerButton.Text = "...";
                            MapHandler.MakeMap();
                            NetworkHandler.SendMap();
                        }
                        else
                            MessageBox.Show("Not enough players to start the game!", "erawrrrr", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        break;
                }
            }
        }
    }
}
