using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    class Enemy
    {
        public Animation EnemyAnimation;
        public Vector2 Position;

        public bool Active;
        public int Health;
        public int Damage;
        public int Value; // value of hitting enemy
        public int Width => EnemyAnimation.FrameWidth;
        public int height => EnemyAnimation.FrameHeight;
        private float enemyMoveSpeed;

        public void Initialize(Animation animation, Vector2 position)
        {
            EnemyAnimation = animation;

            Position = position;

            Active = true;

            Health = 10;

            Damage = 10;

            enemyMoveSpeed = 6f;

            Value = 100;
        }

        public void Update(GameTime gameTime)
        {
            Position.X -= enemyMoveSpeed; // always move to the left

            EnemyAnimation.Position = Position;
            EnemyAnimation.Update(gameTime);

            if (Position.X < -Width || Health <= 0)
            {
                Active = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            EnemyAnimation.Draw(spriteBatch);
        }
    }
}
