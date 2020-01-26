using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    internal class Player
    {
        // Animation representing the player - texture abstraction
        public Animation PlayerAnimation;

        // position of the player, relative to upper left of screen
        public Vector2 Position;

        // State of the player
        public bool Active { get; set; }

        // Amount of hit points the player has
        public int Health { get; set; }

        // Width of the player 
        public int Width => PlayerAnimation.FrameWidth;

        // Height of the player's image
        public int Height => PlayerAnimation.FrameHeight;

        public void Initialize(Animation animation, Vector2 position)
        {
            // This abstracts the Texture, which is presented by this object 
            PlayerAnimation = animation;
            Position = position;
            Active = true;
            Health = 100;

        }

        public void Update(GameTime gameTime)
        {
            // pass the player's position to the animation, so that the animation can draw to that position
            PlayerAnimation.Position = Position;

            // Update the animation if needed
            PlayerAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            // We've delegated drawing the player to the PlayerAnimation object
            PlayerAnimation.Draw(spritebatch);
        }

    }
}
