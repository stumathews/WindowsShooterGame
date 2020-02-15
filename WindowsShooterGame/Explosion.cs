using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class Explosion
    {
        public Animation explosionAnimation;
        private Vector2 Postion;
        public bool Active;
        public int timeToLive;
        public int Width => explosionAnimation.FrameWidth;
        public int Height => explosionAnimation.FrameHeight;

        public void Initialize(Animation animation, Vector2 position)
        {
            explosionAnimation = animation;
            Postion = position;
            Active = true;
            timeToLive = 30;
        }

        public void Update(GameTime gameTime)
        {
            explosionAnimation.Update(gameTime);
            timeToLive -= 1;
            if (timeToLive <= 0) Active = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            explosionAnimation.Draw(spriteBatch);
        }
    }
}