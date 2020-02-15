using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WindowsShooterGame
{
    public class Animation
    {
        private bool _active;
        private Color _color;
        private int _currentFrame;
        private Rectangle _destinationRect;
        private int _elapsedTime;
        private int _frameCount;
        public int FrameHeight;
        private readonly int _frameTime;
        public int FrameWidth;
        private bool _looping;
        public Vector2 Position;
        private float _scale;
        private Rectangle _sourceRect;
        private Texture2D _spriteStrip;

        public Animation(){ }

        public void Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount,
            Color color, float scale, bool looping)
        {
            _color = color;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            _frameCount = frameCount;
            _scale = scale;
            _looping = looping;
            Position = position;
            _spriteStrip = texture;
            _elapsedTime = 0;
            _currentFrame = 0;
            _active = true;
        }

        /// <summary>
        ///     (uses the elapsed time to determine to switch the destination rect on the image strip)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (_active == false) return;

            _elapsedTime += (int) gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_elapsedTime > _frameTime)
            {
                _currentFrame++;
                if (_currentFrame == _frameCount)
                {
                    _currentFrame = 0;
                    if (_looping == false) _active = false;
                }

                _elapsedTime = 0;
            }

            _sourceRect = new Rectangle(_currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);

            _destinationRect = new Rectangle(
                (int) Position.X - (int) (FrameWidth * _scale) / 2,
                (int) Position.Y - (int) (FrameHeight * _scale) / 2,
                (int) (FrameWidth * _scale),
                (int) (FrameHeight * _scale));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_active)
                spriteBatch.Draw(_spriteStrip,  _destinationRect, _sourceRect , _color);
        }
    }
}