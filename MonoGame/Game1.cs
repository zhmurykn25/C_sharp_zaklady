using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Palky
        private Rectangle player1;
        private Rectangle player2;
        private float player1Speed = 300f;
        private float player2Speed = 250f;

        // Míč
        private Rectangle ball;
        private Vector2 ballVelocity;
        private float ballSpeed = 250f;

        // Skóre
        private int player1Score = 0;
        private int player2Score = 0;

        // Font pro skóre
        private SpriteFont font;
        private Texture2D pixel;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Inicializace pálky hráče 1 (levá)
            player1 = new Rectangle(20, 250, 15, 100);

            // Inicializace pálky hráče 2 (pravá - AI)
            player2 = new Rectangle(765, 250, 15, 100);

            // Inicializace míče
            ball = new Rectangle(400, 300, 10, 10);
            ballVelocity = new Vector2(-ballSpeed, -ballSpeed * 0.5f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Vytvoříme bílý pixel pro kreslení obdélníků
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Pokusíme se načíst font (není povinný)
            try
            {
                font = Content.Load<SpriteFont>("Arial");
            }
            catch
            {
                font = null; // Pokud font neexistuje, budeme kreslit bez skóre
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();

            // Ovládání hráče 1 (W/S nebo Up/Down)
            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            {
                player1.Y -= (int)(player1Speed * deltaTime);
            }
            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            {
                player1.Y += (int)(player1Speed * deltaTime);
            }

            // Omezení hráče 1 v okrajích
            if (player1.Y < 0) player1.Y = 0;
            if (player1.Y + player1.Height > GraphicsDevice.Viewport.Height)
                player1.Y = GraphicsDevice.Viewport.Height - player1.Height;

            // AI pro hráče 2 - sleduje míč s určitou zpožděním
            float ai2TargetY = ball.Y - player2.Height / 2;
            if (player2.Y < ai2TargetY - 10)
            {
                player2.Y += (int)(player2Speed * deltaTime);
            }
            else if (player2.Y > ai2TargetY + 10)
            {
                player2.Y -= (int)(player2Speed * deltaTime);
            }

            // Omezení hráče 2 v okrajích
            if (player2.Y < 0) player2.Y = 0;
            if (player2.Y + player2.Height > GraphicsDevice.Viewport.Height)
                player2.Y = GraphicsDevice.Viewport.Height - player2.Height;

            // Pohyb míče
            ball.X += (int)(ballVelocity.X * deltaTime);
            ball.Y += (int)(ballVelocity.Y * deltaTime);

            // Odskok od horního a dolního okraje
            if (ball.Y <= 0 || ball.Y >= GraphicsDevice.Viewport.Height - ball.Height)
            {
                ballVelocity.Y *= -1;
            }

            // Odskok od pálky hráče 1
            if (ball.Intersects(player1))
            {
                ballVelocity.X *= -1;
                ball.X = player1.Right;
                // Přidej rotaci podle pozice na páce
                float hitPos = (ball.Y - player1.Y) / (float)player1.Height;
                ballVelocity.Y = (hitPos - 0.5f) * ballSpeed * 1.5f;
            }

            // Odskok od pálky hráče 2
            if (ball.Intersects(player2))
            {
                ballVelocity.X *= -1;
                ball.X = player2.Left - ball.Width;
                // Přidej rotaci podle pozice na páce
                float hitPos = (ball.Y - player2.Y) / (float)player2.Height;
                ballVelocity.Y = (hitPos - 0.5f) * ballSpeed * 1.5f;
            }

            // Kontrola gólů
            if (ball.X < 0)
            {
                player2Score++;
                ResetBall();
            }
            if (ball.X > GraphicsDevice.Viewport.Width)
            {
                player1Score++;
                ResetBall();
            }

            base.Update(gameTime);
        }

        private void ResetBall()
        {
            ball.X = GraphicsDevice.Viewport.Width / 2;
            ball.Y = GraphicsDevice.Viewport.Height / 2;
            ballVelocity.X = ballSpeed * (ball.X % 2 == 0 ? 1 : -1);
            ballVelocity.Y = ballSpeed * 0.3f * (ball.Y % 2 == 0 ? 1 : -1);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // Kreslení středové čáry (přerušované)
            for (int i = 0; i < GraphicsDevice.Viewport.Height; i += 20)
            {
                _spriteBatch.Draw(pixel, new Rectangle(GraphicsDevice.Viewport.Width / 2, i, 2, 10), Color.White);
            }

            // Kreslení pálky hráče 1
            _spriteBatch.Draw(pixel, player1, Color.White);

            // Kreslení pálky hráče 2
            _spriteBatch.Draw(pixel, player2, Color.White);

            // Kreslení míče
            _spriteBatch.Draw(pixel, ball, Color.White);

            // Kreslení skóre
            if (font != null)
            {
                string score = $"{player1Score}     {player2Score}";
                Vector2 scorePosition = new Vector2(GraphicsDevice.Viewport.Width / 2 - 30, 20);
                _spriteBatch.DrawString(font, score, scorePosition, Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
