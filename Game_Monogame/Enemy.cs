using Microsoft.Xna.Framework;

namespace Game_Monogame
{
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
}
