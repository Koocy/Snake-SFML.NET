using System.Windows.Forms;
using System.Drawing;
using System;

namespace Snake_SFML.NET
{
    public partial class SettingsMenu : Form
    {
        public Button startButton;
        public Button closeButton;
        public Button leftArrow;
        public Label speed;
        public Button rightArrow;
        public CheckBox botCB;

        void closeSettingsMenu(Game gameRef)
        {
            if (gameRef.gameStarted)
            {
                this.Hide();
            }
            else Application.Exit();
        }

        void increaseSpeed(Game gameRef)
        {
            if (speed.Text == "Medium") { gameRef.newMoveDelay -= 0.1f; speed.Text = "Fast"; return; }
            else if (speed.Text == "Slow") { gameRef.newMoveDelay -= 0.1f; speed.Text = "Medium"; return; }
            else if (speed.Text == "Fast") { gameRef.newMoveDelay += 0.2f; speed.Text = "Slow"; return; }
        }

        void decreaseSpeed(Game gameRef)
        {
            if (speed.Text == "Medium") { gameRef.newMoveDelay += 0.1f; speed.Text = "Slow"; return; }
            else if (speed.Text == "Slow") { gameRef.newMoveDelay -= 0.2f; speed.Text = "Fast"; return; }
            else if (speed.Text == "Fast") { gameRef.newMoveDelay += 0.1f; speed.Text = "Medium"; return; }
        
        }

        void StartOrRestartGame(Game gameRef)
        {
            gameRef.moveDelay = gameRef.newMoveDelay;
            gameRef.bot = botCB.Checked;
            this.Hide();

            if (gameRef.gameStarted)
            {
                if (MessageBox.Show("Start new game?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    gameRef.RestartGame();
                }
            }
            else if (!gameRef.gameStarted)
            {
                closeButton.Text = "CANCEL";
                gameRef.Run();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Enter:
                    startButton.PerformClick();
                    return true;

                case Keys.Escape:
                    closeButton.PerformClick();
                    return true;

                case Keys.Left:
                case Keys.A:
                    leftArrow.PerformClick();
                    return true;

                case Keys.Right:
                case Keys.D:
                    rightArrow.PerformClick();
                    return true;

                case Keys.Tab:
                case Keys.B:
                    botCB.Checked = !botCB.Checked;
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        public SettingsMenu(Game gameRef)
            : base()
        {
            this.DoubleBuffered = true;

            this.FormClosed += (s, e) => Environment.Exit(0);

            startButton = new Button();
            closeButton = new Button();
            leftArrow = new Button();
            speed = new Label();
            rightArrow = new Button();
            botCB = new CheckBox();

            this.ClientSize = new Size(800, 600);
            this.Text = "Settings";
            this.ControlBox = false;

            startButton.TabStop = false;
            startButton.Size = new Size(150, 50);
            startButton.BackColor = Color.Green;
            startButton.Left = this.ClientSize.Width / 2 + 10;
            startButton.Top = ((this.ClientSize.Height * 3) / 4) - startButton.Size.Height / 2;
            startButton.FlatAppearance.BorderColor = Color.Black;
            startButton.FlatAppearance.BorderSize = 3;
            startButton.Text = "START";
            startButton.ForeColor = Color.White;

            closeButton.TabStop = false;
            closeButton.Size = new Size(150, 50);
            closeButton.BackColor = Color.DarkRed;
            closeButton.Left = this.ClientSize.Width / 2 - 10 - closeButton.Size.Width;
            closeButton.Top = ((this.ClientSize.Height * 3) / 4) - startButton.Size.Height / 2;
            closeButton.FlatAppearance.BorderColor = Color.Black;
            closeButton.FlatAppearance.BorderSize = 3;
            closeButton.Text = "EXIT";
            closeButton.ForeColor = Color.White;

            speed.AutoSize = true;
            speed.Left = this.ClientSize.Width / 2 - speed.Size.Width / 2;
            speed.Top = startButton.Top - 80;
            speed.Text = "Medium";
            speed.TextAlign = ContentAlignment.MiddleCenter;
            speed.Font = new Font(new FontFamily("Arial"), 12f);

            botCB.TabStop = false;
            botCB.Left = speed.Left;
            botCB.Top = speed.Top - 10 - botCB.Size.Height;
            botCB.Text = "Bot";
            botCB.Font = new Font(new FontFamily("Arial"), 12f);

            leftArrow.TabStop = false;
            leftArrow.Size = new Size(40, 40);
            leftArrow.BackColor = Color.Cyan;
            leftArrow.Left = speed.Left - 40 - leftArrow.Size.Width;
            leftArrow.Top = speed.Top;
            leftArrow.FlatAppearance.BorderColor = Color.Black;
            leftArrow.FlatAppearance.BorderSize = 1;
            leftArrow.Text = "<";

            rightArrow.TabStop = false;
            rightArrow.Size = new Size(40, 40);
            rightArrow.BackColor = Color.Cyan;
            rightArrow.Left = speed.Right + 40;
            rightArrow.Top = speed.Top;
            rightArrow.FlatAppearance.BorderColor = Color.Black;
            rightArrow.FlatAppearance.BorderSize = 1;
            rightArrow.Text = ">";

            Controls.Add(startButton);
            Controls.Add(closeButton);
            Controls.Add(leftArrow);
            Controls.Add(speed);
            Controls.Add(rightArrow);
            Controls.Add(botCB);

            startButton.Click += (s, e) => { StartOrRestartGame(gameRef); };
            closeButton.Click += (s, e) => { closeSettingsMenu(gameRef); };
            leftArrow.Click += (s, e) => { decreaseSpeed(gameRef); };
            rightArrow.Click += (s, e) => { increaseSpeed(gameRef); };
        }
    }
}