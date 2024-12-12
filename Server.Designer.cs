namespace Blue_Lagoon___Chaos_Edition__SERVER_ {
    partial class Server {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            BackgroundPanel = new TableLayoutPanel();
            ConfigPanel = new TableLayoutPanel();
            AddBotButton = new Button();
            tableLayoutPanel5 = new TableLayoutPanel();
            MapSizeBoxBackground = new Panel();
            MapSizeBox = new TextBox();
            MapSizeLabel = new Label();
            PlayerListPanel = new TableLayoutPanel();
            ImportantPanel = new TableLayoutPanel();
            ServerButton = new Button();
            ServerPortBoxBackground = new Panel();
            ServerPortBox = new TextBox();
            BackgroundPanel.SuspendLayout();
            ConfigPanel.SuspendLayout();
            tableLayoutPanel5.SuspendLayout();
            MapSizeBoxBackground.SuspendLayout();
            ImportantPanel.SuspendLayout();
            ServerPortBoxBackground.SuspendLayout();
            SuspendLayout();
            // 
            // BackgroundPanel
            // 
            BackgroundPanel.BackColor = Color.FromArgb(40, 40, 40);
            BackgroundPanel.ColumnCount = 2;
            BackgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            BackgroundPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            BackgroundPanel.Controls.Add(ConfigPanel, 1, 0);
            BackgroundPanel.Controls.Add(PlayerListPanel, 0, 0);
            BackgroundPanel.Controls.Add(ImportantPanel, 1, 1);
            BackgroundPanel.Dock = DockStyle.Fill;
            BackgroundPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            BackgroundPanel.Location = new Point(0, 0);
            BackgroundPanel.Margin = new Padding(0);
            BackgroundPanel.Name = "BackgroundPanel";
            BackgroundPanel.RowCount = 2;
            BackgroundPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            BackgroundPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            BackgroundPanel.Size = new Size(624, 321);
            BackgroundPanel.TabIndex = 0;
            // 
            // ConfigPanel
            // 
            ConfigPanel.ColumnCount = 1;
            ConfigPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            ConfigPanel.Controls.Add(AddBotButton, 0, 1);
            ConfigPanel.Controls.Add(tableLayoutPanel5, 0, 0);
            ConfigPanel.Dock = DockStyle.Fill;
            ConfigPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            ConfigPanel.Location = new Point(318, 4);
            ConfigPanel.Margin = new Padding(6, 4, 6, 4);
            ConfigPanel.Name = "ConfigPanel";
            ConfigPanel.RowCount = 3;
            ConfigPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            ConfigPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            ConfigPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            ConfigPanel.Size = new Size(300, 248);
            ConfigPanel.TabIndex = 2;
            // 
            // AddBotButton
            // 
            AddBotButton.BackColor = Color.FromArgb(30, 30, 30);
            AddBotButton.Dock = DockStyle.Fill;
            AddBotButton.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            AddBotButton.FlatAppearance.BorderSize = 2;
            AddBotButton.FlatStyle = FlatStyle.Flat;
            AddBotButton.Font = new Font("Segoe UI", 20F);
            AddBotButton.ForeColor = Color.White;
            AddBotButton.Location = new Point(6, 66);
            AddBotButton.Margin = new Padding(6, 4, 6, 4);
            AddBotButton.Name = "AddBotButton";
            AddBotButton.Size = new Size(288, 54);
            AddBotButton.TabIndex = 4;
            AddBotButton.Text = "Add Bot";
            AddBotButton.UseVisualStyleBackColor = false;
            AddBotButton.Click += AddBotButton_Click;
            // 
            // tableLayoutPanel5
            // 
            tableLayoutPanel5.ColumnCount = 2;
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Controls.Add(MapSizeBoxBackground, 0, 0);
            tableLayoutPanel5.Controls.Add(MapSizeLabel, 0, 0);
            tableLayoutPanel5.Dock = DockStyle.Fill;
            tableLayoutPanel5.Location = new Point(3, 3);
            tableLayoutPanel5.Name = "tableLayoutPanel5";
            tableLayoutPanel5.RowCount = 1;
            tableLayoutPanel5.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel5.Size = new Size(294, 56);
            tableLayoutPanel5.TabIndex = 3;
            // 
            // MapSizeBoxBackground
            // 
            MapSizeBoxBackground.BackColor = Color.FromArgb(30, 30, 30);
            MapSizeBoxBackground.BorderStyle = BorderStyle.FixedSingle;
            MapSizeBoxBackground.Controls.Add(MapSizeBox);
            MapSizeBoxBackground.Dock = DockStyle.Fill;
            MapSizeBoxBackground.ForeColor = Color.FromArgb(34, 34, 34);
            MapSizeBoxBackground.Location = new Point(151, 6);
            MapSizeBoxBackground.Margin = new Padding(4, 6, 4, 6);
            MapSizeBoxBackground.Name = "MapSizeBoxBackground";
            MapSizeBoxBackground.Size = new Size(139, 44);
            MapSizeBoxBackground.TabIndex = 4;
            // 
            // MapSizeBox
            // 
            MapSizeBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            MapSizeBox.BackColor = Color.FromArgb(30, 30, 30);
            MapSizeBox.BorderStyle = BorderStyle.None;
            MapSizeBox.Font = new Font("Segoe UI", 23F);
            MapSizeBox.ForeColor = Color.White;
            MapSizeBox.Location = new Point(-1, 1);
            MapSizeBox.Margin = new Padding(0);
            MapSizeBox.MaxLength = 3;
            MapSizeBox.Name = "MapSizeBox";
            MapSizeBox.PlaceholderText = "14";
            MapSizeBox.Size = new Size(140, 41);
            MapSizeBox.TabIndex = 2;
            MapSizeBox.Text = "14";
            MapSizeBox.TextAlign = HorizontalAlignment.Center;
            MapSizeBox.WordWrap = false;
            // 
            // MapSizeLabel
            // 
            MapSizeLabel.Dock = DockStyle.Fill;
            MapSizeLabel.Font = new Font("Segoe UI", 20F);
            MapSizeLabel.ForeColor = Color.White;
            MapSizeLabel.Location = new Point(4, 6);
            MapSizeLabel.Margin = new Padding(4, 6, 4, 6);
            MapSizeLabel.Name = "MapSizeLabel";
            MapSizeLabel.Size = new Size(139, 44);
            MapSizeLabel.TabIndex = 3;
            MapSizeLabel.Text = "Map Size";
            MapSizeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PlayerListPanel
            // 
            PlayerListPanel.AutoScroll = true;
            PlayerListPanel.ColumnCount = 1;
            PlayerListPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            PlayerListPanel.Dock = DockStyle.Fill;
            PlayerListPanel.Location = new Point(6, 4);
            PlayerListPanel.Margin = new Padding(6, 4, 6, 4);
            PlayerListPanel.Name = "PlayerListPanel";
            PlayerListPanel.RowCount = 1;
            PlayerListPanel.RowStyles.Add(new RowStyle());
            PlayerListPanel.Size = new Size(300, 248);
            PlayerListPanel.TabIndex = 1;
            // 
            // ImportantPanel
            // 
            ImportantPanel.ColumnCount = 2;
            ImportantPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ImportantPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            ImportantPanel.Controls.Add(ServerButton, 1, 0);
            ImportantPanel.Controls.Add(ServerPortBoxBackground, 0, 0);
            ImportantPanel.Dock = DockStyle.Fill;
            ImportantPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            ImportantPanel.Location = new Point(318, 260);
            ImportantPanel.Margin = new Padding(6, 4, 6, 4);
            ImportantPanel.Name = "ImportantPanel";
            ImportantPanel.RowCount = 1;
            ImportantPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            ImportantPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 49F));
            ImportantPanel.Size = new Size(300, 57);
            ImportantPanel.TabIndex = 0;
            // 
            // ServerButton
            // 
            ServerButton.BackColor = Color.FromArgb(30, 30, 30);
            ServerButton.Dock = DockStyle.Fill;
            ServerButton.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            ServerButton.FlatAppearance.BorderSize = 2;
            ServerButton.FlatStyle = FlatStyle.Flat;
            ServerButton.Font = new Font("Segoe UI", 18F);
            ServerButton.ForeColor = Color.White;
            ServerButton.Location = new Point(156, 4);
            ServerButton.Margin = new Padding(6, 4, 6, 4);
            ServerButton.Name = "ServerButton";
            ServerButton.Size = new Size(138, 49);
            ServerButton.TabIndex = 0;
            ServerButton.Text = "Run Server";
            ServerButton.UseVisualStyleBackColor = false;
            ServerButton.Click += ServerButton_Click;
            // 
            // ServerPortBoxBackground
            // 
            ServerPortBoxBackground.BackColor = Color.FromArgb(30, 30, 30);
            ServerPortBoxBackground.BorderStyle = BorderStyle.FixedSingle;
            ServerPortBoxBackground.Controls.Add(ServerPortBox);
            ServerPortBoxBackground.Dock = DockStyle.Fill;
            ServerPortBoxBackground.ForeColor = Color.FromArgb(34, 34, 34);
            ServerPortBoxBackground.Location = new Point(4, 4);
            ServerPortBoxBackground.Margin = new Padding(4);
            ServerPortBoxBackground.Name = "ServerPortBoxBackground";
            ServerPortBoxBackground.Size = new Size(142, 49);
            ServerPortBoxBackground.TabIndex = 1;
            // 
            // ServerPortBox
            // 
            ServerPortBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ServerPortBox.BackColor = Color.FromArgb(30, 30, 30);
            ServerPortBox.BorderStyle = BorderStyle.None;
            ServerPortBox.Font = new Font("Segoe UI", 23F);
            ServerPortBox.ForeColor = Color.White;
            ServerPortBox.Location = new Point(0, 4);
            ServerPortBox.Margin = new Padding(0);
            ServerPortBox.MaxLength = 5;
            ServerPortBox.Name = "ServerPortBox";
            ServerPortBox.PlaceholderText = "Port";
            ServerPortBox.Size = new Size(140, 41);
            ServerPortBox.TabIndex = 1;
            ServerPortBox.Text = "60420";
            ServerPortBox.TextAlign = HorizontalAlignment.Center;
            ServerPortBox.WordWrap = false;
            ServerPortBox.TextChanged += ServerPortBox_TextChanged;
            // 
            // Server
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(624, 321);
            Controls.Add(BackgroundPanel);
            Margin = new Padding(6, 4, 6, 4);
            MinimumSize = new Size(640, 360);
            Name = "Server";
            Text = "Blue Lagoon - Chaos Edition (SERVER)";
            Load += Server_Load;
            Resize += Form_Resize;
            BackgroundPanel.ResumeLayout(false);
            ConfigPanel.ResumeLayout(false);
            tableLayoutPanel5.ResumeLayout(false);
            MapSizeBoxBackground.ResumeLayout(false);
            MapSizeBoxBackground.PerformLayout();
            ImportantPanel.ResumeLayout(false);
            ServerPortBoxBackground.ResumeLayout(false);
            ServerPortBoxBackground.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        internal TableLayoutPanel BackgroundPanel;
        internal TableLayoutPanel ImportantPanel;
        internal Button ServerButton;
        internal TextBox ServerPortBox;
        internal Panel ServerPortBoxBackground;
        internal TableLayoutPanel PlayerListPanel;
        internal TableLayoutPanel ConfigPanel;
        private TableLayoutPanel tableLayoutPanel5;
        internal Panel MapSizeBoxBackground;
        internal TextBox MapSizeBox;
        private Label MapSizeLabel;
        internal Button AddBotButton;
    }
}
