using System.Windows.Forms;
using System.Drawing;
using System;

namespace Snake
{
    public partial class SettingsMenu : Form
    {
        public Button startButton;
        public Button closeButton;
        public Button leftArrow;
        public Label speed;
        public Button rightArrow;
        public CheckBox botCB;

        public SettingsMenu()
            : base()
        {
            this.DoubleBuffered = true;

            this.FormClosed += (s, e) => Environment.Exit(0);

            this.KeyPreview = true;

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

            this.Controls.Add(startButton);
            this.Controls.Add(closeButton);
            this.Controls.Add(leftArrow);
            this.Controls.Add(speed);
            this.Controls.Add(rightArrow);
            this.Controls.Add(botCB);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Enter:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

    }
}