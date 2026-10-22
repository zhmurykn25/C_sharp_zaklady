using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game_Monogame
{
    // Třída pro projektil
    public class Bullet
    {
        public Rectangle Rect;
        public Vector2 Velocity;
        public bool IsActive;

        public Bullet(Rectangle rect, Vector2 velocity)
        {
            Rect = rect;
            Velocity = velocity;
            IsActive = true;
        }
    }

    // Třída pro nepřítele
    public class Enemy
    {
        public Rectangle Rect;
        public Vector2 Velocity;
        public bool IsActive;
        public int Health;

        public Enemy(Rectangle rect, Vector2 velocity, int health = 1)
        {
            Rect = rect;
            Velocity = velocity;
            IsActive = true;
            Health = health;
        }
    }

    // Třída pro částice (efekty)
    public class Particle
    {
        public Rectangle Rect;
        public Vector2 Velocity;
        public float LifeTime;
        public float MaxLifeTime;
        public bool IsActive;
        public Color Color;

        public Particle(Rectangle rect, Vector2 velocity, float lifetime, Color color)
        {
            Rect = rect;
            Velocity = velocity;
            LifeTime = lifetime;
            MaxLifeTime = lifetime;
            IsActive = true;
            Color = color;
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D pixel;

        // Hráč
        private Rectangle player;
        private const float PlayerSpeed = 350f;
        private const int PlayerWidth = 30;
        private const int PlayerHeight = 40;

        // Kulky
        private List<Bullet> bullets;
        private const float BulletSpeed = 500f;
        private float shootCooldown = 0f;
        private const float ShootDelay = 0.15f;

        // Nepřátelé
        private List<Enemy> enemies;
        private Random random;
        private float spawnCooldown = 0f;
        private float spawnRate = 0.8f;
        private float difficultyTimer = 0f;

        // Částice
        private List<Particle> particles;

        // UI
        private int score = 0;
        private int health = 100;
        private int level = 1;
        private int enemiesDefeated = 0;
        private GameState gameState = GameState.Playing;
        private float gameOverTimer = 0f;

        // Font
        private SpriteFont font;

        // Waves
        private int enemiesInWave = 5;
        private int enemiesSpawned = 0;
        private int enemiesKilled = 0;

        private enum GameState { Playing, GameOver }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 700;
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            // Inicializace hráče (uprostřed dole)
            player = new Rectangle(
                _graphics.PreferredBackBufferWidth / 2 - PlayerWidth / 2,
                _graphics.PreferredBackBufferHeight - 60,
                PlayerWidth,
                PlayerHeight
            );

            bullets = new List<Bullet>();
            enemies = new List<Enemy>();
            particles = new List<Particle>();
            random = new Random();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Vytvoříme bílý pixel pro kreslení
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            // Pokus se načíst font
            try
            {
                font = Content.Load<SpriteFont>("Arial");
            }
            catch
            {
                font = null;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
                Exit();

            if (gameState == GameState.GameOver)
            {
                gameOverTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (keyboard.IsKeyDown(Keys.Space))
                {
                    ResetGame();
                }
                base.Update(gameTime);
                return;
            }

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Pohyb hráče
            if (keyboard.IsKeyDown(Keys.Left) || keyboard.IsKeyDown(Keys.A))
            {
                player.X -= (int)(PlayerSpeed * deltaTime);
            }
            if (keyboard.IsKeyDown(Keys.Right) || keyboard.IsKeyDown(Keys.D))
            {
                player.X += (int)(PlayerSpeed * deltaTime);
            }

            // Omezení hráče na obrazovku
            if (player.X < 0) player.X = 0;
            if (player.X + player.Width > GraphicsDevice.Viewport.Width)
                player.X = GraphicsDevice.Viewport.Width - player.Width;

            // Střelba
            shootCooldown -= deltaTime;
            if ((keyboard.IsKeyDown(Keys.Space) || keyboard.IsKeyDown(Keys.Up)) && shootCooldown <= 0)
            {
                Shoot();
                shootCooldown = ShootDelay;
            }

            // Pohyb kulek
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Rect.Y -= (int)(BulletSpeed * deltaTime);
                if (bullets[i].Rect.Y < 0)
                    bullets.RemoveAt(i);
            }

            // Spawn nepřátel
            spawnCooldown -= deltaTime;
            if (spawnCooldown <= 0 && enemiesSpawned < enemiesInWave)
            {
                SpawnEnemy();
                spawnCooldown = spawnRate;
                enemiesSpawned++;
            }

            // Pohyb nepřátel
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Rect.X += (int)(enemies[i].Velocity.X * deltaTime);
                enemies[i].Rect.Y += (int)(enemies[i].Velocity.Y * deltaTime);

                // Kolize s hráčem
                if (enemies[i].Rect.Intersects(player))
                {
                    health -= 10;
                    CreateExplosion(enemies[i].Rect.Center.ToVector2(), Color.OrangeRed);
                    enemies.RemoveAt(i);
                    continue;
                }

                // Odebrání se obrazovky
                if (enemies[i].Rect.Y > GraphicsDevice.Viewport.Height)
                {
                    enemies.RemoveAt(i);
                }
            }

            // Kolize kulek s nepřáteli
            for (int b = bullets.Count - 1; b >= 0; b--)
            {
                for (int e = enemies.Count - 1; e >= 0; e--)
                {
                    if (bullets[b].Rect.Intersects(enemies[e].Rect))
                    {
                        enemies[e].Health--;
                        CreateExplosion(enemies[e].Rect.Center.ToVector2(), Color.Yellow);

                        if (enemies[e].Health <= 0)
                        {
                            score += 100;
                            enemiesKilled++;
                            enemies.RemoveAt(e);
                        }

                        bullets.RemoveAt(b);
                        break;
                    }
                }
            }

            // Aktualizace částic
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].LifeTime -= deltaTime;
                particles[i].Rect.X += (int)(particles[i].Velocity.X * deltaTime);
                particles[i].Rect.Y += (int)(particles[i].Velocity.Y * deltaTime);

                if (particles[i].LifeTime <= 0)
                    particles.RemoveAt(i);
            }

            // Obtížnost
            difficultyTimer += deltaTime;
            if (difficultyTimer > 10f)
            {
                level++;
                enemiesInWave += 2;
                spawnRate = Math.Max(0.3f, spawnRate - 0.1f);
                difficultyTimer = 0f;
            }

            // Nová vlna
            if (enemiesSpawned >= enemiesInWave && enemies.Count == 0)
            {
                enemiesSpawned = 0;
                enemiesInWave += 1;
                level++;
            }

            // Game Over
            if (health <= 0)
            {
                gameState = GameState.GameOver;
            }

            base.Update(gameTime);
        }

        private void Shoot()
        {
            Rectangle bulletRect = new Rectangle(
                player.X + player.Width / 2 - 2,
                player.Y - 10,
                4,
                15
            );
            bullets.Add(new Bullet(bulletRect, Vector2.Zero));

            // Efekt střelby
            CreateExplosion(new Vector2(player.X + player.Width / 2, player.Y), Color.LimeGreen);
        }

        private void SpawnEnemy()
        {
            int x = random.Next(30, GraphicsDevice.Viewport.Width - 30);
            int enemyType = random.Next(3);
            int health = 1 + (level / 3);

            Rectangle enemyRect = new Rectangle(x, -40, 35, 35);
            Vector2 velocity = new Vector2(
                (float)(random.NextDouble() - 0.5) * 100,
                150 + level * 10
            );

            enemies.Add(new Enemy(enemyRect, velocity, health));
        }

        private void CreateExplosion(Vector2 position, Color color)
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = (float)Math.PI * 2 * i / 8;
                Vector2 velocity = new Vector2(
                    (float)Math.Cos(angle) * 150,
                    (float)Math.Sin(angle) * 150
                );
                particles.Add(new Particle(
                    new Rectangle((int)position.X, (int)position.Y, 4, 4),
                    velocity,
                    0.5f,
                    color
                ));
            }
        }

        private void ResetGame()
        {
            score = 0;
            health = 100;
            level = 1;
            enemiesDefeated = 0;
            spawnRate = 0.8f;
            difficultyTimer = 0f;
            enemiesInWave = 5;
            enemiesSpawned = 0;
            enemiesKilled = 0;
            gameState = GameState.Playing;
            gameOverTimer = 0f;

            bullets.Clear();
            enemies.Clear();
            particles.Clear();

            Initialize();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(15, 15, 35));

            _spriteBatch.Begin();

            // Hvězdy na pozadí
            for (int i = 0; i < 50; i++)
            {
                _spriteBatch.Draw(pixel, new Rectangle(
                    (int)((i * 97) % GraphicsDevice.Viewport.Width),
                    (int)((i * 157) % GraphicsDevice.Viewport.Height),
                    1,
                    1
                ), Color.White * 0.3f);
            }

            // Kreslení hráče
            _spriteBatch.Draw(pixel, player, Color.Cyan);

            // Kreslení kulek
            foreach (var bullet in bullets)
            {
                _spriteBatch.Draw(pixel, bullet.Rect, Color.LimeGreen);
            }

            // Kreslení nepřátel
            foreach (var enemy in enemies)
            {
                _spriteBatch.Draw(pixel, enemy.Rect, Color.Red);
                // Zdraví nepřítele
                if (enemy.Health > 1)
                {
                    _spriteBatch.Draw(pixel, new Rectangle(enemy.Rect.X, enemy.Rect.Y - 5, enemy.Rect.Width, 2), Color.LimeGreen);
                }
            }

            // Kreslení částic
            foreach (var particle in particles)
            {
                float alpha = particle.LifeTime / particle.MaxLifeTime;
                _spriteBatch.Draw(pixel, particle.Rect, particle.Color * alpha);
            }

            // UI
            if (font != null)
            {
                _spriteBatch.DrawString(font, $"Score: {score}", new Vector2(20, 20), Color.White);
                _spriteBatch.DrawString(font, $"Health: {health}", new Vector2(20, 50), Color.Lerp(Color.LimeGreen, Color.Red, 1 - (health / 100f)));
                _spriteBatch.DrawString(font, $"Wave: {level}", new Vector2(20, 80), Color.Yellow);
                _spriteBatch.DrawString(font, $"Defeated: {enemiesKilled}", new Vector2(20, 110), Color.Cyan);
            }

            // Game Over Screen
            if (gameState == GameState.GameOver)
            {
                _spriteBatch.Draw(pixel, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Black * 0.6f);

                if (font != null)
                {
                    string gameOverText = "GAME OVER";
                    string finalScore = $"Final Score: {score}";
                    string wavesText = $"Waves Survived: {level}";
                    string restartText = "Press SPACE to restart";

                    Vector2 centerX = new Vector2(GraphicsDevice.Viewport.Width / 2, 0);

                    Vector2 gameOverSize = font.MeasureString(gameOverText);
                    _spriteBatch.DrawString(font, gameOverText, centerX - gameOverSize / 2 + new Vector2(0, 150), Color.Red);

                    Vector2 finalScoreSize = font.MeasureString(finalScore);
                    _spriteBatch.DrawString(font, finalScore, centerX - finalScoreSize / 2 + new Vector2(0, 220), Color.Yellow);

                    Vector2 wavesSize = font.MeasureString(wavesText);
                    _spriteBatch.DrawString(font, wavesText, centerX - wavesSize / 2 + new Vector2(0, 270), Color.Cyan);

                    Vector2 restartSize = font.MeasureString(restartText);
                    _spriteBatch.DrawString(font, restartText, centerX - restartSize / 2 + new Vector2(0, 350), Color.White);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
