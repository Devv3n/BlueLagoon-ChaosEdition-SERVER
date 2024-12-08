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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            MapSizeBoxBackground = new Panel();
            MapSizeBox = new TextBox();
            MapSizeLabel = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            ServerButton = new Button();
            ServerPortBoxBackground = new Panel();
            ServerPortBox = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            MapSizeBoxBackground.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ServerPortBoxBackground.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 1, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.Size = new Size(624, 321);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel4.Controls.Add(MapSizeBoxBackground, 1, 1);
            tableLayoutPanel4.Controls.Add(MapSizeLabel, 0, 1);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel4.Location = new Point(318, 4);
            tableLayoutPanel4.Margin = new Padding(6, 4, 6, 4);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 2;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 75F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel4.Size = new Size(300, 248);
            tableLayoutPanel4.TabIndex = 2;
            // 
            // MapSizeBoxBackground
            // 
            MapSizeBoxBackground.BackColor = Color.White;
            MapSizeBoxBackground.BorderStyle = BorderStyle.FixedSingle;
            MapSizeBoxBackground.Controls.Add(MapSizeBox);
            MapSizeBoxBackground.Dock = DockStyle.Fill;
            MapSizeBoxBackground.Location = new Point(154, 192);
            MapSizeBoxBackground.Margin = new Padding(4, 6, 4, 6);
            MapSizeBoxBackground.Name = "MapSizeBoxBackground";
            MapSizeBoxBackground.Size = new Size(142, 50);
            MapSizeBoxBackground.TabIndex = 1;
            // 
            // MapSizeBox
            // 
            MapSizeBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            MapSizeBox.BackColor = Color.White;
            MapSizeBox.BorderStyle = BorderStyle.None;
            MapSizeBox.Font = new Font("Segoe UI", 23F);
            MapSizeBox.Location = new Point(0, 4);
            MapSizeBox.Margin = new Padding(0);
            MapSizeBox.MaxLength = 3;
            MapSizeBox.Name = "MapSizeBox";
            MapSizeBox.PlaceholderText = "14";
            MapSizeBox.Size = new Size(140, 41);
            MapSizeBox.TabIndex = 2;
            MapSizeBox.Text = "14";
            MapSizeBox.TextAlign = HorizontalAlignment.Center;
            MapSizeBox.WordWrap = false;
            MapSizeBox.TextChanged += MapSizeBox_TextChanged;
            // 
            // MapSizeLabel
            // 
            MapSizeLabel.Dock = DockStyle.Fill;
            MapSizeLabel.Font = new Font("Segoe UI", 20F);
            MapSizeLabel.Location = new Point(4, 192);
            MapSizeLabel.Margin = new Padding(4, 6, 4, 6);
            MapSizeLabel.Name = "MapSizeLabel";
            MapSizeLabel.Size = new Size(142, 50);
            MapSizeLabel.TabIndex = 2;
            MapSizeLabel.Text = "Map Size";
            MapSizeLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 1;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Dock = DockStyle.Fill;
            tableLayoutPanel3.Location = new Point(6, 4);
            tableLayoutPanel3.Margin = new Padding(6, 4, 6, 4);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle());
            tableLayoutPanel3.Size = new Size(300, 248);
            tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Controls.Add(ServerButton, 1, 0);
            tableLayoutPanel2.Controls.Add(ServerPortBoxBackground, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
            tableLayoutPanel2.Location = new Point(318, 260);
            tableLayoutPanel2.Margin = new Padding(6, 4, 6, 4);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 49F));
            tableLayoutPanel2.Size = new Size(300, 57);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // ServerButton
            // 
            ServerButton.Dock = DockStyle.Fill;
            ServerButton.Font = new Font("Segoe UI", 18F);
            ServerButton.Location = new Point(156, 4);
            ServerButton.Margin = new Padding(6, 4, 6, 4);
            ServerButton.Name = "ServerButton";
            ServerButton.Size = new Size(138, 49);
            ServerButton.TabIndex = 0;
            ServerButton.Text = "Run Server";
            ServerButton.UseVisualStyleBackColor = true;
            ServerButton.Click += ServerButton_Click;
            // 
            // ServerPortBoxBackground
            // 
            ServerPortBoxBackground.BackColor = Color.White;
            ServerPortBoxBackground.BorderStyle = BorderStyle.FixedSingle;
            ServerPortBoxBackground.Controls.Add(ServerPortBox);
            ServerPortBoxBackground.Dock = DockStyle.Fill;
            ServerPortBoxBackground.Location = new Point(4, 4);
            ServerPortBoxBackground.Margin = new Padding(4);
            ServerPortBoxBackground.Name = "ServerPortBoxBackground";
            ServerPortBoxBackground.Size = new Size(142, 49);
            ServerPortBoxBackground.TabIndex = 1;
            // 
            // ServerPortBox
            // 
            ServerPortBox.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            ServerPortBox.BackColor = Color.White;
            ServerPortBox.BorderStyle = BorderStyle.None;
            ServerPortBox.Font = new Font("Segoe UI", 23F);
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
            ClientSize = new Size(624, 321);
            Controls.Add(tableLayoutPanel1);
            Margin = new Padding(6, 4, 6, 4);
            MinimumSize = new Size(640, 360);
            Name = "Server";
            Text = "Blue Lagoon - Chaos Edition (SERVER)";
            Resize += Form_Resize;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            MapSizeBoxBackground.ResumeLayout(false);
            MapSizeBoxBackground.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ServerPortBoxBackground.ResumeLayout(false);
            ServerPortBoxBackground.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        internal TableLayoutPanel tableLayoutPanel1;
        internal TableLayoutPanel tableLayoutPanel2;
        internal Button ServerButton;
        internal TextBox ServerPortBox;
        internal Panel ServerPortBoxBackground;
        internal TableLayoutPanel tableLayoutPanel3;
        internal TableLayoutPanel tableLayoutPanel4;
        internal Panel MapSizeBoxBackground;
        internal TextBox MapSizeBox;
        private Label MapSizeLabel;
    }
}
