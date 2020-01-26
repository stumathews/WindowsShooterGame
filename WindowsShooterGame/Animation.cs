using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content; using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class Animation
    {
        public Texture2D SpriteStrip;
        public float Scale;
        public int ElapsedTime;
        public int FrameTime;
        public int FrameCount;
        public int CurrentFrame;
        public Color Colour;
        Rectangle SourceRect = new Rectangle();
        Rectangle DestinationRect = new Rectangle();
        public int FrameWidth;
        public int FrameHeight;
        public bool Active;
        public bool Looping;
        public Vector2 Position;

        public void Initialize(Texture2D texture, Vector2 position,
            int frameWidth, int frameHeight, int frameCount,
            Color color, float scale, bool looping)
        {
            this.Colour = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.FrameCount = frameCount;
            this.Scale = scale;
            this.Looping = looping;
            this.Position = position;
            this.SpriteStrip = texture;
            this.ElapsedTime = 0;
            this.CurrentFrame = 0;

            this.Active = true;
        }
        /// <summary>
        ///  (uses the elapsed time to determine to switch the destination rect on the image strip)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (Active == false) return;

            ElapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (ElapsedTime > FrameTime)
            {
                CurrentFrame++;
                if (CurrentFrame == FrameCount)
                {
                    CurrentFrame = 0;
                    if (Looping == false)
                    {
                        Active = false;
                    }
                }

                ElapsedTime = 0;
            }

            // Were would the current frame's rectangle be?
            SourceRect = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            // This is where we'll be putting it - use player's position to derive this
            DestinationRect = new Rectangle(
                (int) Position.X - (int) (FrameWidth * Scale) / 2,
                (int) Position.Y - (int) (FrameHeight * Scale) / 2,
                (int) (FrameWidth * Scale),
                (int) (FrameHeight * Scale));

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                spriteBatch.Draw(SpriteStrip, DestinationRect/*where on the screen should i draw it - player position is used to derive this */, SourceRect/*where on the strip is current frame*/, Colour);
            }
        }
    }
}
