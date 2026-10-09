using Raylib_cs;
using System.Numerics;

namespace Pong
{
    internal class Program
    {
        const int SCREEN_WIDTH = 1000;
        const int SCREEN_HEIGHT = 600;

        static void Main(string[] args)
        {
            Raylib.InitWindow(SCREEN_WIDTH, SCREEN_HEIGHT, "🏓 PONG");
            Raylib.SetTargetFPS(60);

            // Paddles
            Paddle playerPaddle = new Paddle(20, SCREEN_HEIGHT / 2 - 50, 15, 100);
            Paddle computerPaddle = new Paddle(SCREEN_WIDTH - 35, SCREEN_HEIGHT / 2 - 50, 15, 100);

            // Ball
            Ball ball = new Ball(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2, 10);

            // Score
            int playerScore = 0;
            int computerScore = 0;

            while (!Raylib.WindowShouldClose())
            {
                // Input
                playerPaddle.Update(SCREEN_HEIGHT);

                // Computer AI
                if (ball.Y < computerPaddle.Y + computerPaddle.Height / 2)
                    computerPaddle.Y -= 4;
                else
                    computerPaddle.Y += 4;

                // Clamp computer paddle
                computerPaddle.Y = Math.Clamp(computerPaddle.Y, 0, SCREEN_HEIGHT - computerPaddle.Height);

                // Ball update
                ball.Update();

                // Collision with paddles
                if (ball.CheckCollision(playerPaddle))
                {
                    ball.VelocityX = Math.Abs(ball.VelocityX);
                    ball.X = playerPaddle.X + playerPaddle.Width + ball.Radius;
                    ball.VelocityY += (ball.Y - (playerPaddle.Y + playerPaddle.Height / 2)) * 0.1f;
                }

                if (ball.CheckCollision(computerPaddle))
                {
                    ball.VelocityX = -Math.Abs(ball.VelocityX);
                    ball.X = computerPaddle.X - ball.Radius;
                    ball.VelocityY += (ball.Y - (computerPaddle.Y + computerPaddle.Height / 2)) * 0.1f;
                }

                // Ball out of bounds - scoring
                if (ball.X < 0)
                {
                    computerScore++;
                    ball.Reset(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
                }
                else if (ball.X > SCREEN_WIDTH)
                {
                    playerScore++;
                    ball.Reset(SCREEN_WIDTH / 2, SCREEN_HEIGHT / 2);
                }

                // Ball collision with top/bottom
                if (ball.Y - ball.Radius < 0 || ball.Y + ball.Radius > SCREEN_HEIGHT)
                {
                    ball.VelocityY = -ball.VelocityY;
                    ball.Y = Math.Clamp(ball.Y, ball.Radius, SCREEN_HEIGHT - ball.Radius);
                }

                // Drawing
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black);

                // Background grid
                DrawGrid();

                // Center line
                for (int y = 0; y < SCREEN_HEIGHT; y += 20)
                {
                    Raylib.DrawRectangle(SCREEN_WIDTH / 2 - 2, y, 4, 10, Color.White);
                }

                // Paddles
                playerPaddle.Draw();
                computerPaddle.Draw();

                // Ball
                ball.Draw();

                // Score
                Raylib.DrawText(playerScore.ToString(), SCREEN_WIDTH / 4 - 30, 50, 80, Color.RayWhite);
                Raylib.DrawText(computerScore.ToString(), (3 * SCREEN_WIDTH) / 4 - 30, 50, 80, Color.RayWhite);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void DrawGrid()
        {
            for (int x = 0; x < SCREEN_WIDTH; x += 50)
            {
                for (int y = 0; y < SCREEN_HEIGHT; y += 50)
                {
                    Raylib.DrawRectangleLines(x, y, 50, 50, new Color(50, 50, 50, 255));
                }
            }
        }
    }

    class Paddle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        private const float SPEED = 6f;

        public Paddle(float x, float y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void Update(int screenHeight)
        {
            if (Raylib.IsKeyDown(KeyboardKey.W))
                Y -= SPEED;
            if (Raylib.IsKeyDown(KeyboardKey.S))
                Y += SPEED;

            // Clamp to screen
            Y = Math.Clamp(Y, 0, screenHeight - Height);
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)X, (int)Y, Width, Height, Color.RayWhite);
            Raylib.DrawRectangleLines((int)X, (int)Y, Width, Height, Color.Yellow);
        }
    }

    class Ball
    {
        public float X { get; set; }
        public float Y { get; set; }
        public int Radius { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
        private float initialX;
        private float initialY;

        public Ball(float x, float y, int radius)
        {
            X = x;
            Y = y;
            initialX = x;
            initialY = y;
            Radius = radius;
            VelocityX = 5f;
            VelocityY = 4f;
        }

        public void Update()
        {
            X += VelocityX;
            Y += VelocityY;
        }

        public bool CheckCollision(Paddle paddle)
        {
            return Raylib.CheckCollisionCircleRec(
                new Vector2(X, Y),
                Radius,
                new Rectangle(paddle.X, paddle.Y, paddle.Width, paddle.Height)
            );
        }

        public void Reset(float x, float y)
        {
            X = x;
            Y = y;
            VelocityX = (new Random().Next(0, 2) == 0 ? -1 : 1) * 5f;
            VelocityY = (float)(new Random().NextDouble() - 0.5f) * 4f;
        }

        public void Draw()
        {
            Raylib.DrawRectangle((int)(X - Radius), (int)(Y - Radius), Radius * 2, Radius * 2, Color.Lime);
            Raylib.DrawRectangleLines((int)(X - Radius), (int)(Y - Radius), Radius * 2, Radius * 2, Color.Green);
        }
    }
}
