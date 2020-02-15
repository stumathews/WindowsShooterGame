using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class Laser
    {
        public Animation LaserAnimation;
        public float laserMoveSpeed = 30f;
        public Vector2 Position;
        public int Damage = 10;
        public bool Active;
        private int Range;
        public int Width => LaserAnimation.FrameWidth;
        public int Height => LaserAnimation.FrameHeight;


        public void Initialize(Animation animation, Vector2 position)
        {
            LaserAnimation = animation;
            Position = position;
            Active = true;
        }

        public void Update(GameTime gameTime)
        {
            Position.X += laserMoveSpeed;
            LaserAnimation.Position = Position;
            LaserAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            LaserAnimation.Draw(spriteBatch);
        }
    }
}