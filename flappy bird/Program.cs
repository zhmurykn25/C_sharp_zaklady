using Raylib_cs;
using System.Numerics;

namespace flappy_bird
{
    internal class Program
    {
        const int SCREEN_WIDTH = 800;
        const int SCREEN_HEIGHT = 600;
        const int PIPE_WIDTH = 80;
        const int PIPE_GAP = 150;
        const float GRAVITY = 0.6f;
        const float JUMP_POWER = -12f;

        static void Main(string[] args)
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "🐦 Flappy Bird");
            Raylib.SetTargetFPS(60);

            // Hráč (pták)
            Bird bird = new Bird(SCREEN_WIDTH / 4, SCREEN_HEIGHT / 2);

            // Trubky
            List<Pipe> pipes = new List<Pipe>();
            float spawnTimer = 0f;
            float spawnInterval = 2.5f;

            // Skóre
            int score = 0;
            int bestScore = 0;
            bool gameOver = false;
            float gameOverTimer = 0f;

            // Inicializace prvního setu trubek
            pipes.Add(new Pipe());

            while (!Raylib.WindowShouldClose())
            {
                // Logika
                if (!gameOver)
                {
                    // Pohyb ptáka
                    bird.Update();

                    // Skákání
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        bird.Jump();
                    }

                    // Spawn trubek
                    spawnTimer += Raylib.GetFrameTime();
                    if (spawnTimer >= spawnInterval)
                    {
                        pipes.Add(new Pipe());
                        spawnTimer = 0f;
                    }

                    // Pohyb trubek a kontrola kolizí
                    for (int i = pipes.Count - 1; i >= 0; i--)
                    {
                        pipes[i].Update();

                        // Kontrola skóre
                        if (pipes[i].X + PIPE_WIDTH == (int)bird.X && !pipes[i].Scored)
                        {
                            score++;
                            if (score > bestScore)
                                bestScore = score;
                            pipes[i].Scored = true;
                        }

                        // Kontrola kolizí
                        if (bird.CheckCollision(pipes[i]))
                        {
                            gameOver = true;
                            gameOverTimer = 0f;
                        }

                        // Odebrání trubek mimo obrazovku
                        if (pipes[i].X < -PIPE_WIDTH)
                        {
                            pipes.RemoveAt(i);
                        }
                    }

                    // Kontrola kolize se stěnami
                    if (bird.Y - bird.Radius < 0 || bird.Y + bird.Radius > SCREEN_HEIGHT)
                    {
                        gameOver = true;
                        gameOverTimer = 0f;
                    }
                }
                else
                {
                    gameOverTimer += Raylib.GetFrameTime();

                    // Game Over - restart na mezerník
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                    {
                        gameOver = false;
                        score = 0;
                        bird.Reset();
                        pipes.Clear();
                        pipes.Add(new Pipe());
                        spawnTimer = 0f;
                    }
                }

                // Vykreslování
                Raylib.BeginDrawing();

                // Gradient pozadí
                DrawGradientBackground();

                // Mraky
                DrawClouds();

                // Trubky
                foreach (var pipe in pipes)
                {
                    pipe.Draw();
                }

                // Pták
                bird.Draw();

                // UI - Skóre
                Raylib.DrawText($"SKÓRE: {score}", 30, 30, 35, Color.White);
                Raylib.DrawRectangleLines(20, 20, 200, 50, Color.White);

                Raylib.DrawText($"BEST: {bestScore}", 620, 30, 35, Color.White);
                Raylib.DrawRectangleLines(610, 20, 170, 50, Color.White);

                // Game Over
                if (gameOver && gameOverTimer > 0.5f)
                {
                    DrawGameOverScreen(score, bestScore);
                }

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void DrawGradientBackground()
        {
            // Horní část - světlá modrá
            for (int y = 0; y < SCREEN_HEIGHT / 2; y++)
            {
                float gradient = y / (float)(SCREEN_HEIGHT / 2);
                Color color = new Color(
                    (int)(135 + gradient * 30),
                    (int)(206 - gradient * 50),
                    (int)(235 - gradient * 50),
                    255
                );
                Raylib.DrawLine(0, y, SCREEN_WIDTH, y, color);
            }

            // Dolní část - světlá hnědá (zem)
            for (int y = SCREEN_HEIGHT / 2; y < SCREEN_HEIGHT; y++)
            {
                float gradient = (y - SCREEN_HEIGHT / 2) / (float)(SCREEN_HEIGHT / 2);
                Color color = new Color(
                    (int)(200 - gradient * 50),
                    (int)(150 + gradient * 30),
                    (int)(100 + gradient * 20),
                    255
                );
                Raylib.DrawLine(0, y, SCREEN_WIDTH, y, color);
            }
        }

        static void DrawClouds()
        {
            // Velké mraky vlevo
            DrawCloud(80, 70, 40, new Color(255, 255, 255, 200));
            DrawCloud(150, 120, 35, new Color(240, 245, 255, 180));

            // Středem
            DrawCloud(500, 100, 50, new Color(255, 255, 255, 190));
            DrawCloud(650, 80, 40, new Color(235, 240, 250, 170));

            // Vpravo dole
            DrawCloud(200, 450, 45, new Color(245, 250, 255, 180));
            DrawCloud(650, 480, 35, new Color(240, 245, 255, 160));
        }

        static void DrawCloud(float x, float y, float size, Color color)
        {
            Raylib.DrawCircle((int)(x - size), (int)y, size, color);
            Raylib.DrawCircle((int)x, (int)(y - size * 0.3f), (int)(size * 1.2f), color);
            Raylib.DrawCircle((int)(x + size), (int)y, size, color);
            Raylib.DrawCircle((int)(x + size * 0.5f), (int)(y + size * 0.2f), (int)(size * 0.8f), color);
        }

        static void DrawGameOverScreen(int score, int bestScore)
        {
            // Tmavý overlay
            Raylib.DrawRectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, new Color(0, 0, 0, 150));

            // Game Over box
            int boxWidth = 400;
            int boxHeight = 300;
            int boxX = (SCREEN_WIDTH - boxWidth) / 2;
            int boxY = (SCREEN_HEIGHT - boxHeight) / 2;

            Raylib.DrawRectangle(boxX, boxY, boxWidth, boxHeight, new Color(50, 50, 50, 255));
            Raylib.DrawRectangleLines(boxX, boxY, boxWidth, boxHeight, Color.Gold);

            Raylib.DrawText("GAME OVER!", boxX + 50, boxY + 20, 50, Color.Red);
            Raylib.DrawText($"Skóre: {score}", boxX + 80, boxY + 90, 35, Color.Yellow);
            Raylib.DrawText($"Best: {bestScore}", boxX + 95, boxY + 140, 30, Color.Green);
            Raylib.DrawText("Stiskni SPACE", boxX + 70, boxY + 210, 25, Color.White);
            Raylib.DrawText("pro restart", boxX + 85, boxY + 245, 25, Color.White);
        }
    }

    class Bird
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Radius { get; set; }
        private float velocity = 0f;
        private const float GRAVITY = 0.6f;
        private const float JUMP_POWER = -12f;
        private float rotation = 0f;
        private float initialX;
        private float initialY;

        public Bird(float startX, float startY)
        {
            X = startX;
            Y = startY;
            initialX = startX;
            initialY = startY;
            Radius = 15;
        }

        public void Update()
        {
            velocity += GRAVITY;
            Y += velocity;

            // Rotace ptáka podle směru
            if (velocity < 0)
                rotation = -30;
            else if (velocity > 5)
                rotation = 30;
            else
                rotation += (0 - rotation) * 0.1f;
        }

        public void Jump()
        {
            velocity = JUMP_POWER;
        }

        public void Reset()
        {
            X = initialX;
            Y = initialY;
            velocity = 0f;
            rotation = 0f;
        }

        public bool CheckCollision(Pipe pipe)
        {
            // Horní trubka
            if (Raylib.CheckCollisionCircleRec(new Vector2(X, Y), Radius,
                new Rectangle(pipe.X, 0, Pipe.WIDTH, pipe.TopHeight)))
                return true;

            // Dolní trubka
            if (Raylib.CheckCollisionCircleRec(new Vector2(X, Y), Radius,
                new Rectangle(pipe.X, pipe.TopHeight + Pipe.GAP, Pipe.WIDTH, 600 - (pipe.TopHeight + Pipe.GAP))))
                return true;

            return false;
        }

        public void Draw()
        {
            // Tělo ptáka - gradient
            Raylib.DrawCircle((int)X, (int)Y, Radius, Color.Gold);

            // Tmavší kruh pro střed
            Raylib.DrawCircle((int)X, (int)Y, Radius - 3, Color.Orange);

            // Oči
            int eyeX = (int)(X + 5);
            int eyeY = (int)(Y - 4);
            Raylib.DrawCircle(eyeX, eyeY, 4, Color.White);
            Raylib.DrawCircle(eyeX, eyeY, 2, Color.Black);

            // Zobák
            Raylib.DrawTriangle(
                new Vector2(X + Radius - 3, Y),
                new Vector2(X + Radius + 6, Y - 3),
                new Vector2(X + Radius + 6, Y + 3),
                Color.Red
            );

            // Obrys
            Raylib.DrawCircleLines((int)X, (int)Y, Radius, Color.DarkBrown);
        }
    }

    class Pipe
    {
        public float X { get; set; }
        public int TopHeight { get; set; }
        public bool Scored { get; set; }
        public const int WIDTH = 80;
        public const int GAP = 150;
        private const float SPEED = 5f;

        public Pipe()
        {
            X = 800;
            TopHeight = new Random().Next(100, 350);
            Scored = false;
        }

        public void Update()
        {
            X -= SPEED;
        }

        public void Draw()
        {
            // Horní trubka - tmavě zelená s gradientem
            DrawPipeSegment((int)X, 0, TopHeight, true);

            // Dolní trubka - tmavě zelená s gradientem
            int bottomY = TopHeight + GAP;
            DrawPipeSegment((int)X, bottomY, 600 - bottomY, false);

            // Zdobení - obrysy
            Raylib.DrawRectangleLines((int)X, 0, WIDTH, TopHeight, Color.DarkGreen);
            Raylib.DrawRectangleLines((int)X, bottomY, WIDTH, 600 - bottomY, Color.DarkGreen);
        }

        static void DrawPipeSegment(int x, int y, int height, bool isTop)
        {
            // Hlavní obdélník
            Raylib.DrawRectangle(x + 2, y + 2, WIDTH - 4, height - 4, Color.Green);

            // Vnitřní dekorace - "dřevo"
            for (int i = 0; i < height; i += 15)
            {
                Raylib.DrawLine(x + 5, y + i, x + WIDTH - 5, y + i, new Color(0, 100, 0, 150));
            }

            // Okraj - metalická barva
            int rimHeight = isTop ? 15 : 15;
            Color rimColor = isTop ? Color.Gold : Color.Gold;

            if (isTop)
            {
                Raylib.DrawRectangle(x + 5, y + height - rimHeight, WIDTH - 10, rimHeight, rimColor);
                Raylib.DrawRectangleLines(x + 5, y + height - rimHeight, WIDTH - 10, rimHeight, Color.Orange);
            }
            else
            {
                Raylib.DrawRectangle(x + 5, y, WIDTH - 10, rimHeight, rimColor);
                Raylib.DrawRectangleLines(x + 5, y, WIDTH - 10, rimHeight, Color.Orange);
            }

            // Stín pro hloubku
            Raylib.DrawRectangleLines(x, y, WIDTH, height, new Color(0, 0, 0, 100));
        }
    }
}