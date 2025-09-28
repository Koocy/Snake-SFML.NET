using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace Snake
{
    class Program
    {
        const int SW_MAXIMIZE = 3;
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr HWND, int nCmdCommand);  
        [DllImport("user32.dll")]
        static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        static Font Arial = new Font(@"C:\Windows\Fonts\arial.ttf");
        static Text pauseText = new Text("Press 'P' or Enter to start/unpause", Arial, 40);

        static RenderWindow wnd;
        static Clock clock;
        static List<Drawable> toDraw;

        static float moveDelay = 0.4f;
        static float timer;

        static int appleRadius = 10;
        static CircleShape apple;
        static Vector2i applePosition;

        static List<RectangleShape> snake;
        static List<Vector2i> snakePositions;
        static int unit = 40;

        static Vector2i currentDirection;
        static List<Vector2i> directionQ;

        static bool pause;
        static bool gameOver;
        static bool showMessage;

        static int gridW; //48
        static int gridH; //27

        static void InitGame()
        {
            toDraw = new List<Drawable>();

            pauseText.FillColor = Color.Black;
            toDraw.Add(pauseText);

            clock = new Clock();

            snake = new List<RectangleShape>();
            snakePositions = new List<Vector2i>();

            currentDirection = new Vector2i(1, 0);
            directionQ = new List<Vector2i>();
            directionQ.Add(currentDirection);

            timer = 0.0f;

            pause = true;
            gameOver = false;
            showMessage = false;

            wnd = new RenderWindow(new VideoMode(1920, 1080, VideoMode.DesktopMode.BitsPerPixel), "Snake", Styles.Close);
            wnd.SetFramerateLimit(60);
            wnd.Closed += closeWindow;
            wnd.KeyPressed += Wnd_KeyPressed;

            ShowWindow(wnd.SystemHandle, SW_MAXIMIZE);

            gridW = (int)(wnd.Size.X / unit);
            gridH = (int)(wnd.Size.Y / unit);

            for (int i = 0; i < 3; i++)
            {
                Color fill;
                if (i == 0) fill = Color.Black; else fill = Color.Cyan;

                RectangleShape rect = new RectangleShape((unit, unit))
                {
                    FillColor = fill,
                    OutlineColor = Color.Black,
                    OutlineThickness = 2,
                    Position = (unit * (2 - i), unit)
                };

                snake.Add(rect);
                snakePositions.Add(new Vector2i((2 - i), 1));
                toDraw.Add(rect);
            }

            applePosition = new Vector2i(0, 0);
        }

        static void generateApple()
        {
            Random random = new Random();
        start:
            int appleX = random.Next(0, gridW);
            Thread.Sleep(20);
            int appleY = random.Next(0, gridH);
            for (int i = 0; i < snakePositions.Count; i++)
            {
                if (appleX == snakePositions[i].X || appleX == snakePositions[0].X + currentDirection.X) { Thread.Sleep(20);  goto start; }
                if (appleY == snakePositions[i].Y || appleY == snakePositions[0].Y + currentDirection.Y) { Thread.Sleep(20); goto start; }
            }

            applePosition = new Vector2i(appleX, appleY);

            if (apple == null)
                apple = new CircleShape(appleRadius)
                {
                    FillColor = Color.Red,
                    OutlineColor = Color.Black,
                    OutlineThickness = 1,
                    Position = ((applePosition.X * unit) + 12, (applePosition.Y * unit) + 12)
                };
            else apple.Position = ((applePosition.X * unit) + 12, (applePosition.Y * unit) + 12);

            toDraw.Add(apple);
        }

        static RenderWindow settingsMenu;
        static RectangleShape startButton;
        static Text startButtonText;
        static RectangleShape leftArrow;
        static Text leftArrowText;
        static Text speedText;
        static RectangleShape rightArrow;
        static Text rightArrowText;
        static void Main()
        {
            settingsMenu = new RenderWindow(new VideoMode(800, 600), "Settings", Styles.Default);
            settingsMenu.SetFramerateLimit(60);
            settingsMenu.Closed += closeWindow;
            settingsMenu.MouseButtonPressed += Menu_MouseButtonPressed;
            settingsMenu.KeyPressed += Menu_KeyPressed;

            startButton = new RectangleShape((150, 50));
            startButton.FillColor = Color.Green;
            startButton.Position = ((settingsMenu.Size.X / 2 - startButton.Size.X / 2), ((settingsMenu.Size.Y * 3) / 4 - startButton.Size.Y / 2));
            startButton.OutlineColor = Color.Black;
            startButton.OutlineThickness = 3;

            startButtonText = new Text("START", Arial, 30);
            startButtonText.Position = startButton.Position + (26, 5);
            startButtonText.FillColor = Color.Black;

            leftArrow = new RectangleShape((40, 40));
            leftArrow.FillColor = Color.Cyan;
            leftArrow.Position = (startButton.Position.X - 40, startButton.Position.Y - 80);
            leftArrow.OutlineThickness = 1;
            leftArrow.OutlineColor = Color.Black;

            leftArrowText = new Text("<", Arial, 30);
            leftArrowText.Position = leftArrow.Position + (7, 1);
            leftArrowText.FillColor = Color.Black;

            speedText = new Text("Medium", Arial, 30);
            speedText.Position = ((leftArrow.Position.X + leftArrow.Size.X + 20), (leftArrow.Position.Y));
            speedText.FillColor = Color.Black;

            rightArrow = new RectangleShape((40, 40));
            rightArrow.FillColor = Color.Cyan;
            rightArrow.Position = (startButton.Position.X + startButton.Size.X, startButton.Position.Y - 80);
            rightArrow.OutlineThickness = 1;
            rightArrow.OutlineColor = Color.Black;

            rightArrowText = new Text(">", Arial, 30);
            rightArrowText.Position = rightArrow.Position + (7, 1);
            rightArrowText.FillColor = Color.Black;

            while (settingsMenu.IsOpen)
            {
                settingsMenu.DispatchEvents();
                settingsMenu.Clear(Color.White);
                drawMenuWindow();
                settingsMenu.Display();
            }
        }

        private static void Menu_KeyPressed(object sender, KeyEventArgs e)
        {
            switch(e.Code)
            {
                case Keyboard.Key.Enter:
                    closeWindow(settingsMenu, new EventArgs());
                    Game();
                    break;
                case Keyboard.Key.A:
                case Keyboard.Key.Left:
                        if (speedText.DisplayedString == "Medium") { moveDelay += 0.1f; speedText.DisplayedString = "Slow"; return; }
                        else if (speedText.DisplayedString == "Slow") { moveDelay -= 0.2f; speedText.DisplayedString = "Fast"; return; }
                        else if (speedText.DisplayedString == "Fast") { moveDelay += 0.1f; speedText.DisplayedString = "Medium"; return; }
                    break;
                case Keyboard.Key.D:
                case Keyboard.Key.Right:
                        if (speedText.DisplayedString == "Medium") { moveDelay -= 0.1f; speedText.DisplayedString = "Fast"; return; }
                        else if (speedText.DisplayedString == "Slow") { moveDelay -= 0.1f; speedText.DisplayedString = "Medium"; return; }
                        else if (speedText.DisplayedString == "Fast") { moveDelay += 0.2f; speedText.DisplayedString = "Slow"; return; }
                    break;
            }
        }

        static bool mouseCheck(MouseButtonEventArgs e, RectangleShape rect)
        {
            if (e.X >= rect.Position.X && e.Y >= rect.Position.Y && e.X <= rect.Position.X + rect.Size.X && e.Y <= rect.Position.Y + rect.Size.Y) return true;
            else return false;
        }

        private static void Menu_MouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            if (mouseCheck(e, startButton))
            {
                closeWindow(settingsMenu, new EventArgs());
                Game();
            }
            else if (mouseCheck(e, leftArrow))
            {
                if (speedText.DisplayedString == "Medium") { moveDelay += 0.1f; speedText.DisplayedString = "Slow"; return; }
                else if (speedText.DisplayedString == "Slow") { moveDelay -= 0.2f; speedText.DisplayedString = "Fast"; return; }
                else if (speedText.DisplayedString == "Fast") { moveDelay += 0.1f; speedText.DisplayedString = "Medium"; return; }
            }
            else if (mouseCheck(e, rightArrow))
            {
                if (speedText.DisplayedString == "Medium") { moveDelay -= 0.1f; speedText.DisplayedString = "Fast"; return; }
                else if (speedText.DisplayedString == "Slow") { moveDelay -= 0.1f; speedText.DisplayedString = "Medium"; return; }
                else if (speedText.DisplayedString == "Fast") { moveDelay += 0.2f; speedText.DisplayedString = "Slow"; return; }
            }
        }

        static void Game()
        {
            InitGame();
            generateApple();

            while (wnd.IsOpen)
            {
                wnd.DispatchEvents();

                if (!pause)
                {
                    float deltaTime = clock.Restart().AsSeconds();
                    timer += deltaTime;
                    if (timer >= moveDelay)
                    {
                        timer -= moveDelay;
                        updateSnakePositions();
                    }
                }
                else
                {
                    if (!gameOver) clock.Restart();
                }

                wnd.Clear(Color.White);
                drawGameWindow();
                wnd.Display();

                if (gameOver && showMessage)
                {
                    MessageBox(IntPtr.Zero, "GAME OVER\nPress 'R' to restart", "", 0);
                    showMessage = false;
                }
            }
        }

        static void updateSnakePositions()
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
                if (snakePositions[i].X == -1) snakePositions[i] = ((gridW - 1), snakePositions[i].Y);
                else if (snakePositions[i].X == gridW) snakePositions[i] = (0, snakePositions[i].Y);
                if (snakePositions[i].Y == -1) snakePositions[i] = (snakePositions[i].X, (gridH - 1));
                else if (snakePositions[i].Y == gridH) snakePositions[i] = (snakePositions[i].X, 0);

                snake[i].Position = new Vector2f(
                    snakePositions[i].X * unit,
                    snakePositions[i].Y * unit
                    );
            }

            collisionTest();

            if (directionQ.Count != 0)
            directionQ.RemoveAt(0);

            if (gameOver)
            {
                pause = true;
                snake[0].FillColor = Color.Red;
            }
        }

        static void increaseSnakeLength()
        {
            RectangleShape rect = new RectangleShape((unit, unit))
            {
                FillColor = Color.Cyan,
                OutlineColor = Color.Black,
                OutlineThickness = 2,
                Position = (((snakePositions[snakePositions.Count - 1].X - currentDirection.X) * unit), ((snakePositions[snakePositions.Count - 1].Y - currentDirection.Y) * unit))
            };

                snake.Add(rect);
                snakePositions.Add(new Vector2i((int)(rect.Position.X/unit), (int)(rect.Position.Y/unit)));
                toDraw.Add(rect);
        }

        static void collisionTest()
        {
            for (int i = 1; i < snakePositions.Count; i++)
            {
                if (snakePositions[0].X == snakePositions[i].X && snakePositions[0].Y == snakePositions[i].Y)
                {
                    gameOver = true;
                    showMessage = true;
                }
            }

            if (snakePositions[0].X == applePosition.X && snakePositions[0].Y == applePosition.Y)
            {
                increaseSnakeLength();
                generateApple();
            }
        }

        static void Wnd_KeyPressed(object sender, KeyEventArgs e)
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
                        if (lastDirection.Y == 0) nextDirection = (0, -1);
                        break;

                    case Keyboard.Key.A:
                    case Keyboard.Key.Left:
                        if (lastDirection.X == 0) nextDirection = (-1, 0);
                        break;

                    case Keyboard.Key.S:
                    case Keyboard.Key.Down:
                        if (lastDirection.Y == 0) nextDirection = (0, 1);
                        break;

                    case Keyboard.Key.D:
                    case Keyboard.Key.Right:
                        if (lastDirection.X == 0) nextDirection = (1, 0);
                        break;
                }

                directionQ.Add(nextDirection);
            }
            if (gameOver && e.Code == Keyboard.Key.R)
            {
                if (!showMessage)
                {
                    closeWindow(wnd, new EventArgs());
                    Main();
                }
            }
            if (!gameOver && (e.Code == Keyboard.Key.P || e.Code == Keyboard.Key.Enter)) pause = !pause;
        }

        static void drawMenuWindow()
        {
            settingsMenu.Draw(startButton);
            settingsMenu.Draw(startButtonText);
            settingsMenu.Draw(leftArrow);
            settingsMenu.Draw(leftArrowText);
            settingsMenu.Draw(speedText);
            settingsMenu.Draw(rightArrow);
            settingsMenu.Draw(rightArrowText);
        }

        static void drawGameWindow()
        {
            for (int i = toDraw.Count - 1; i >= 0; i--)
            {
                if (!pause || (pause && gameOver)) if (toDraw[i] == pauseText) continue;

                wnd.Draw(toDraw[i]);
            }
        }

        private static void closeWindow(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}
