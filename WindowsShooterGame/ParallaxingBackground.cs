using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class ParallaxingBackground
    {
        // the background
        public Texture2D texture;
        public Vector2[] positions;
        public int speed; // speed background is moving
        public int bgHeight;
        public int bgWidth;

        public void Initialize(ContentManager content, string texturePath,
            int screenWidth, int screenHeight, int speed)
        {
            bgHeight = screenHeight;
            bgWidth = screenWidth;

            texture = content.Load<Texture2D>(texturePath);

            this.speed = speed;

            var numOfTiles = (int) (Math.Ceiling(screenWidth / (float) texture.Width) + 1);
            positions = new Vector2[numOfTiles];
            for (var i = 0; i < positions.Length; i++) positions[i] = new Vector2(i * texture.Width, 0);
        }

        public void Update(GameTime gameTime)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                // move the position
                positions[i].X += speed;

                //moving left
                if (speed <= 0)
                {
                    if (positions[i].X <= -texture.Width) WrapTextureToLeft(i);
                }
                else
                {
                    if (positions[i].X >= texture.Width + (positions.Length - 1)) WrapTextureToRight(i);
                }
            }
        }

        private void WrapTextureToRight(int index)
        {
            var nextTexture = index + 1;
            if (nextTexture == positions.Length)
                nextTexture = 0;
            positions[index].X = positions[nextTexture].X - texture.Width;
        }

        private void WrapTextureToLeft(int index)
        {
            var prevTexture = index - 1;
            if (prevTexture < 0)
                prevTexture = positions.Length - 1;
            positions[index].X = positions[prevTexture].X + texture.Width;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < positions.Length; i++)
            {
                var rectBg = new Rectangle((int) positions[i].X, (int) positions[i].Y, bgWidth, bgHeight);
                spriteBatch.Draw(texture, rectBg, Color.White);
            }
        }
    }
}