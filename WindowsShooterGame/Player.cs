using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    internal class Player
    {
        private Animation _playerAnimation;
        public Vector2 Position;
        public bool Active { get; set; }
        public int Health { get; set; }
        public int Width => _playerAnimation.FrameWidth;
        public int Height => _playerAnimation.FrameHeight;

        public void Initialize(Animation animation, Vector2 position)
        {
            _playerAnimation = animation;
            Position = position;
            Active = true;
            Health = 100;
        }

        public void Update(GameTime gameTime)
        {
            _playerAnimation.Position = Position;
            _playerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            _playerAnimation.Draw(spritebatch);
        }
    }
}