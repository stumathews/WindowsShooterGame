using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class Player
    {
        // Animation representing the player
        public Texture2D Texture { get; set; }

        // position of the player, relative to upper left of screen
        public Vector2 Position { get; set; }

        // State of the player
        public bool Active { get; set; }

        // Amount of hit points the player has
        public int Health { get; set; }

        // Width of the player 
        public int Width => Texture.Width;

        // Height of the player's image
        public int Height => Texture.Height;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            Active = true;
            Health = 100;

        }
        public void Update (){ }

        public void Draw(SpriteBatch spritebatch)
        {
            var drawPosition = new Vector2(x: Position.X, y: Position.Y);
            spritebatch.Draw(Texture, drawPosition, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
