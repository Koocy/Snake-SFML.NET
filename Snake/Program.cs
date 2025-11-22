using System;
using System.Collections.Generic;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Snake
{
    class Program
    {
        static Program Game = new Program();
        static bool gameStarted = false;

        [DllImport("user32.dll")]
        static extern bool SetProcessDPIAware();

        static Font Arial = new Font(@"C:\Windows\Fonts\arial.ttf");
        Text text;

        static RenderWindow GameWindow;
        static SettingsMenu settingsMenu;
        Clock clock;
        List<Drawable> toDraw;

        static float moveDelay = 0.4f;
        static float newMoveDelay = moveDelay;
        float timer;

        CircleShape apple;
        Vector2i applePosition;

        List<RectangleShape> snake;
        List<Vector2i> snakePositions;

        Vector2i currentDirection;
        List<Vector2i> directionQ;

        bool pause;
        bool gameOver;

        const int tileSize = 40;
        const int appleRadius = tileSize / 4;
        const int gridW = 16;
        const int gridH = 9;

        const uint virtualWidth = tileSize * gridW;
        const uint virtualHeight = tileSize * gridH;

        static void Main()
        {
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            settingsMenu = new SettingsMenu();
            settingsMenu.KeyDown += SettingsMenu_KeyDown;
            settingsMenu.startButton.Click += StartButton_Click;
            settingsMenu.closeButton.Click += CloseButton_Click;
            settingsMenu.leftArrow.Click += LeftArrow_Click;
            settingsMenu.rightArrow.Click += RightArrow_Click;

            Application.Run(settingsMenu);
        }

        private static void CloseButton_Click(object sender, EventArgs e)
        {
            if (gameStarted)
            {
                settingsMenu.Hide();
                Game.pause = false;
            }
            else Application.Exit();
        }

        private static void RightArrow_Click(object sender, EventArgs e)
        {
            if (settingsMenu.speed.Text == "Medium") { newMoveDelay -= 0.1f; settingsMenu.speed.Text = "Fast"; return; }
            else if (settingsMenu.speed.Text == "Slow") { newMoveDelay -= 0.1f; settingsMenu.speed.Text = "Medium"; return; }
            else if (settingsMenu.speed.Text == "Fast") { newMoveDelay += 0.2f; settingsMenu.speed.Text = "Slow"; return; }
        }

        private static void LeftArrow_Click(object sender, EventArgs e)
        {
            if (settingsMenu.speed.Text == "Medium") { newMoveDelay += 0.1f; settingsMenu.speed.Text = "Slow"; return; }
            else if (settingsMenu.speed.Text == "Slow") { newMoveDelay -= 0.2f; settingsMenu.speed.Text = "Fast"; return; }
            else if (settingsMenu.speed.Text == "Fast") { newMoveDelay += 0.1f; settingsMenu.speed.Text = "Medium"; return; }
        }

        private static void StartButton_Click(object sender, EventArgs e)
        {
            if (gameStarted)
            {
                if (MessageBox.Show("Start new game?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    moveDelay = newMoveDelay;
                    settingsMenu.Hide();
                    Game.RestartGame();
                }
            }
            else if (!gameStarted)
            {
                moveDelay = newMoveDelay;
                settingsMenu.Hide();
                Game.Run();
            }
        }

        private static void SettingsMenu_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (gameStarted)
                    {
                        if (MessageBox.Show("Start new game?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            moveDelay = newMoveDelay;
                            settingsMenu.Hide();
                            Game.RestartGame();
                        }
                    }
                    else if (!gameStarted)
                    {
                        moveDelay = newMoveDelay;
                        settingsMenu.Hide();
                        Game.Run();
                    }
                    break;
                case Keys.A:
                case Keys.Left:
                    if (settingsMenu.speed.Text == "Medium") { newMoveDelay += 0.1f; settingsMenu.speed.Text = "Slow"; return; }
                    else if (settingsMenu.speed.Text == "Slow") { newMoveDelay -= 0.2f; settingsMenu.speed.Text = "Fast"; return; }
                    else if (settingsMenu.speed.Text == "Fast") { newMoveDelay += 0.1f; settingsMenu.speed.Text = "Medium"; return; }
                    break;
                case Keys.D:
                case Keys.Right:
                    if (settingsMenu.speed.Text == "Medium") { newMoveDelay -= 0.1f; settingsMenu.speed.Text = "Fast"; return; }
                    else if (settingsMenu.speed.Text == "Slow") { newMoveDelay -= 0.1f; settingsMenu.speed.Text = "Medium"; return; }
                    else if (settingsMenu.speed.Text == "Fast") { newMoveDelay += 0.2f; settingsMenu.speed.Text = "Slow"; return; }
                    break;
            }
        }

        void Run()
        {
            gameStarted = true;
            settingsMenu.closeButton.Text = "CANCEL";
            InitGame();
            GenerateApple();
            GameLoop();
        }

        void InitGameWindow()
        {
            GameWindow = new RenderWindow(new VideoMode(virtualWidth, virtualHeight, VideoMode.DesktopMode.BitsPerPixel), "Snake");
            GameWindow.SetFramerateLimit(60);
            GameWindow.Closed += CloseWindow;
            GameWindow.KeyPressed += GameWindow_KeyPressed;
        }

        void MakeSnake()
        {
            Color fill;
            for (int i = 0; i < 3; i++)
            {
                if (i == 0) fill = Color.Black; else fill = Color.Cyan;

                RectangleShape rect = new RectangleShape(new Vector2f(tileSize, tileSize))
                {
                    FillColor = fill,
                    OutlineColor = Color.Black,
                    OutlineThickness = 2,
                    Position = (tileSize * (2 - i), tileSize)
                };

                snake.Add(rect);
                snakePositions.Add(new Vector2i((2 - i), 1));
                toDraw.Add(rect);
            }
        }

        void InitGame()
        {
            toDraw = new List<Drawable>();

            text = new Text();
            text.DisplayedString = "Press 'P' or Enter to start/unpause";
            text.Font = Arial;
            text.CharacterSize = 20;
            text.Position = new Vector2f(0, 0);
            text.FillColor = Color.Black;
            toDraw.Add(text);

            clock = new Clock();

            snake = new List<RectangleShape>();
            snakePositions = new List<Vector2i>();

            currentDirection = new Vector2i(1, 0);
            directionQ = new List<Vector2i>();
            directionQ.Add(currentDirection);

            timer = 0.0f;

            pause = true;
            gameOver = false;

            InitGameWindow();

            MakeSnake();

            applePosition = new Vector2i(0, 0);
        }

        Random random = new Random();
        void GenerateApple()
        {
            int gridSize = gridW * gridH;

        start:
            int appleX = random.Next(0, gridW);
            int appleY = random.Next(0, gridH);

            if (snake.Count < gridSize - 2)
            if (appleX == snakePositions[0].X + currentDirection.X && appleY == snakePositions[0].Y + currentDirection.Y) goto start;

            for (int i = 0; i < snakePositions.Count; i++)
            {
                if (appleX == snakePositions[i].X && appleY == snakePositions[i].Y)
                    goto start;
            }

            applePosition = new Vector2i(appleX, appleY);

            if (apple == null)
            {
                apple = new CircleShape(appleRadius)
                {
                    FillColor = Color.Red,
                    OutlineColor = Color.Black,
                    OutlineThickness = 1,
                    Position = new Vector2f((applePosition.X * tileSize) + 12, (applePosition.Y * tileSize) + 12)
                };
                toDraw.Add(apple);
            }
            else apple.Position = new Vector2f((applePosition.X * tileSize) + 12, (applePosition.Y * tileSize) + 12);
        }

        void GameLoop()
        {
            while (GameWindow.IsOpen)
            {
                GameWindow.DispatchEvents();

                if (!pause)
                {
                    float deltaTime = clock.Restart().AsSeconds();
                    timer += deltaTime;
                    if (timer >= moveDelay)
                    {
                        timer -= moveDelay;
                        StepSnake();
                    }
                }
                else
                {
                    if (!gameOver) clock.Restart();
                }

                GameWindow.Clear(Color.White);
                DrawGameWindow();
                GameWindow.Display();

                if (gameOver)
                {
                    text.DisplayedString = "GAME OVER\nPress 'R' to restart";
                }
            }
        }

        void RestartGame()
        {
            toDraw.Clear();

            text.DisplayedString = "Press 'P' or Enter to start/unpause";
            toDraw.Add(text);

            clock.Restart();

            snake.Clear();
            snakePositions.Clear();

            directionQ.Clear();
            currentDirection = new Vector2i(1, 0);
            directionQ.Add(currentDirection);

            timer = 0.0f;
            pause = true;
            gameOver = false;

            MakeSnake();

            apple = null;
            GenerateApple();
        }

        void StepSnake()
        {
            if (directionQ.Count != 0)
            currentDirection = directionQ[0];

            for (int i = snake.Count - 1; i > 0; i--)
            {
                snakePositions[i] = snakePositions[i - 1];
            }

            snakePositions[0] += currentDirection;

            for (int i = 0; i < snake.Count; i++)
            {
                if (snakePositions[i].X == -1) snakePositions[i] = new Vector2i((gridW - 1), snakePositions[i].Y);
                else if (snakePositions[i].X == gridW) snakePositions[i] = new Vector2i(0, snakePositions[i].Y);
                if (snakePositions[i].Y == -1) snakePositions[i] = new Vector2i(snakePositions[i].X, (gridH - 1));
                else if (snakePositions[i].Y == gridH) snakePositions[i] = new Vector2i(snakePositions[i].X, 0);
            }

            CollisionTest();
            UpdateSnake();

            if (directionQ.Count != 0)
            directionQ.RemoveAt(0);
        }

        void UpdateSnake()
        {
            for (int i = 0; i < snake.Count; i++)
                snake[i].Position = new Vector2f(
                    snakePositions[i].X * tileSize,
                    snakePositions[i].Y * tileSize);
        }

        void IncreaseSnakeLength()
        {
            Vector2i last = snakePositions[snakePositions.Count - 1];
            Vector2i secondLast = snakePositions[snakePositions.Count - 2];
            Vector2i tailDirection = last - secondLast;
            Vector2i tailPosition = last + tailDirection;

            RectangleShape rect = new RectangleShape(new Vector2f(tileSize, tileSize))
            {
                FillColor = Color.Cyan,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Position = new Vector2f(tailPosition.X * tileSize, tailPosition.Y * tileSize)
            };

            snake.Add(rect);
            snakePositions.Add(tailPosition);
            toDraw.Add(rect);
        }

        void CollisionTest()
        {
            for (int i = 1; i < snakePositions.Count; i++)
            {
                if (snakePositions[0].X == snakePositions[i].X && snakePositions[0].Y == snakePositions[i].Y)
                {
                    gameOver = true;
                    pause = true;
                    snake[0].FillColor = Color.Red;
                }
            }

            if (snakePositions[0].X == applePosition.X && snakePositions[0].Y == applePosition.Y)
            {
                IncreaseSnakeLength();
                GenerateApple();
            }
        }

        void GameWindow_KeyPressed(object sender, SFML.Window.KeyEventArgs e)
        {
            if (!gameOver && !pause && directionQ.Count < 3)
            {
                Vector2i lastDirection = currentDirection;
                if (directionQ.Count != 0)
                lastDirection = directionQ[directionQ.Count - 1];
                Vector2i nextDirection = lastDirection;

                switch (e.Code)
                {
                    case Keyboard.Key.W:
                    case Keyboard.Key.Up:
                        if (lastDirection.Y == 0) nextDirection = new Vector2i(0, -1);
                        break;

                    case Keyboard.Key.A:
                    case Keyboard.Key.Left:
                        if (lastDirection.X == 0) nextDirection = new Vector2i(-1, 0);
                        break;

                    case Keyboard.Key.S:
                    case Keyboard.Key.Down:
                        if (lastDirection.Y == 0) nextDirection = new Vector2i(0, 1);
                        break;

                    case Keyboard.Key.D:
                    case Keyboard.Key.Right:
                        if (lastDirection.X == 0) nextDirection = new Vector2i(1, 0);
                        break;
                }

                directionQ.Add(nextDirection);
            }

            if (gameOver && e.Code == Keyboard.Key.R)
            {
                RestartGame();
            }

            if (!gameOver && (e.Code == Keyboard.Key.P || e.Code == Keyboard.Key.Enter)) pause = !pause;

            if (e.Code == Keyboard.Key.Escape)
            {
                pause = true;
                settingsMenu.Show();
            }
        }

        void DrawGameWindow()
        {
            for (int i = toDraw.Count - 1; i >= 0; i--)
            {
                if (!pause)
                {
                    if (toDraw[i] == text) continue;
                }

                GameWindow.Draw(toDraw[i]);
            }
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();

            if (window == GameWindow) Environment.Exit(0);
        }
    }
}
