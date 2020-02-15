using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsShooterGame
{
    /// <summary>
    ///     This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private ParallaxingBackground _bgLayer1;
        private ParallaxingBackground _bgLayer2;
        private GamePadState _currentGamePadState;

        private KeyboardState _currentKeyboardState;
        private MouseState _currentMouseState;
        private List<Enemy> _enemies;

        private TimeSpan _enemySpawnTime;

        private Texture2D _enemyTexture;
        private SoundEffectInstance _explosionEffectInstance;

        private List<Explosion> _explosions;
        private SoundEffect _explosionSound;
        private Texture2D _explosionTexture;
        private SpriteFont _font;

        private Song _gameMusic;
        private GraphicsDeviceManager _graphics;
        private List<Laser> _laserBeams;

        private SoundEffect _laserSound;
        private SoundEffectInstance _laserSoundEffectInstance;
        private TimeSpan _laserSpawnTime;

        private Texture2D _laserTexture;
        private Texture2D _mainBackgroundTexture;
        private Player _player;

        private float _playerMoveSpeed;
        private GamePadState _previousGamePadState;
        private KeyboardState _previousKeyboardState;
        private TimeSpan _previousLaserSpawnTime;
        private MouseState _previousMouseState;
        private TimeSpan _previousSpawnTime;

        private Random _random;
        private int _score;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            const float secondsInMinutes = 60f;
            const float rateOfFire = 200f;

            _score = 0;
            _player = new Player();
            _playerMoveSpeed = 8.0f;
            _bgLayer1 = new ParallaxingBackground();
            _bgLayer2 = new ParallaxingBackground();
            _enemies = new List<Enemy>();
            _previousSpawnTime = TimeSpan.Zero;
            _enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            _random = new Random();
            _laserBeams = new List<Laser>();
            _laserSpawnTime = TimeSpan.FromSeconds(secondsInMinutes / rateOfFire);
            _previousLaserSpawnTime = TimeSpan.Zero;
            _explosions = new List<Explosion>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var playerAnimation = new Animation();
            var playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X,
                GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            playerAnimation.Initialize(Content.Load<Texture2D>("Graphics\\shipAnimation"), Vector2.Zero, 115, 69, 8,
                Color.White, 1f, true);

            _player.Initialize(playerAnimation, playerPosition);

            _bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -1);
            _bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -2);

            _mainBackgroundTexture = Content.Load<Texture2D>("Graphics/mainBackground");
            _enemyTexture = Content.Load<Texture2D>("Graphics/mineAnimation");
            _laserTexture = Content.Load<Texture2D>("Graphics/laser");
            _explosionTexture = Content.Load<Texture2D>("Graphics/explosion");
            _laserSound = Content.Load<SoundEffect>("Sound/laserFire");
            _laserSoundEffectInstance = _laserSound.CreateInstance();
            _explosionSound = Content.Load<SoundEffect>("Sound/explosion");
            _explosionEffectInstance = _explosionSound.CreateInstance();
            _font = Content.Load<SpriteFont>("Graphics/gameFont");
            _gameMusic = Content.Load<Song>("Sound/gameMusic");

            MediaPlayer.Play(_gameMusic);
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _previousKeyboardState = _currentKeyboardState;
            _previousMouseState = _currentMouseState;
            _previousGamePadState = _currentGamePadState;

            _currentGamePadState = GamePad.GetState(PlayerIndex.One);
            _currentKeyboardState = Keyboard.GetState();
            _currentMouseState = Mouse.GetState();

            UpdatePlayer(gameTime);

            _bgLayer1.Update(gameTime);
            _bgLayer2.Update(gameTime);

            UpdateEnemies(gameTime);
            UpdateCollision();

            UpdateLaserBeams(gameTime);
            UpdateExplosions(gameTime);

            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
            _laserSoundEffectInstance.Dispose();
            _explosionEffectInstance.Dispose();
        }

        private void FireLaser(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - _previousLaserSpawnTime > _laserSpawnTime)
            {
                _previousLaserSpawnTime = gameTime.TotalGameTime;
                AddLaser();

                _laserSoundEffectInstance.Play();
            }
        }

        private void AddLaser()
        {
            var laserAnimation = new Animation();
            laserAnimation.Initialize(_laserTexture, _player.Position,
                46, 16, 1, Color.White, 1f, true);
            var laser = new Laser();

            var laserPosition = _player.Position;
            laserPosition.X += 30;

            laser.Initialize(laserAnimation, laserPosition);
            _laserBeams.Add(laser);
        }
        
        private void UpdateEnemies(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - _previousSpawnTime > _enemySpawnTime)
            {
                _previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }

            for (var i = _enemies.Count - 1; i >= 0; i--)
            {
                _enemies[i].Update(gameTime);
                if (_enemies[i].Active) continue;
                _score += _enemies[i].Value;
                _enemies.RemoveAt(i);
            }

            /* local func */
            void AddEnemy()
            {
                var enemyAnimation = new Animation();
                var position = new Vector2(GraphicsDevice.Viewport.Width + _enemyTexture.Width / 2, _random.Next(100, GraphicsDevice.Viewport.Height - 100));
                var enemy = new Enemy();

                enemyAnimation.Initialize(_enemyTexture, Vector2.Zero, 47, 61, 8, Color.White, 1f, true);
                enemy.Initialize(enemyAnimation, position);

                _enemies.Add(enemy);
            }
        }

        private void UpdateCollision()
        {
            var playerRectangle = new Rectangle((int) _player.Position.X,
                (int) _player.Position.Y,
                _player.Width,
                _player.Height);

            for (var i = 0; i < _enemies.Count; i++)
            {
                var enemyRectangle = new Rectangle((int) _enemies[i].Position.X,
                    (int) _enemies[i].Position.Y,
                    _enemies[i].Width,
                    _enemies[i].height);

                if (playerRectangle.Intersects(enemyRectangle))
                {
                    _player.Health -= _enemies[i].Damage;

                    _enemies[i].Health = 0;

                    if (_player.Health <= 0)
                        _player.Active = false;

                    AddExplosion(_enemies[i].Position);

                    _enemies[i].Health = 0;
                }

                for (var l = 0; l < _laserBeams.Count; l++)
                {
                    var laserRectangle = new Rectangle(
                        (int) _laserBeams[l].Position.X,
                        (int) _laserBeams[l].Position.Y,
                        _laserBeams[l].Width,
                        _laserBeams[l].Height);

                    if (laserRectangle.Intersects(enemyRectangle))
                    {
                        _enemies[i].Health = 0;
                        _laserBeams[l].Active = false;
                        if (laserRectangle.Intersects(enemyRectangle))
                        {
                            AddExplosion(_enemies[i].Position);

                            _enemies[i].Health = 0;

                            _laserBeams[l].Active = false;
                        }
                    }
                }
            }
            /* local */
            void AddExplosion(Vector2 enemyPosition)
            {
                var explosionAnimation = new Animation();
                explosionAnimation.Initialize(_explosionTexture,
                    enemyPosition, 134,
                    134,
                    12, Color.White, 1.0f,
                    true);

                var explosion = new Explosion();
                explosion.Initialize(explosionAnimation, enemyPosition);
                _explosions.Add(explosion);
                _explosionEffectInstance.Play();
            }
        }


        private void UpdateExplosions(GameTime gameTime)
        {
            for (var e = _explosions.Count - 1; e >= 0; e--)
            {
                _explosions[e].Update(gameTime);
                if (!_explosions[e].Active)
                    _explosions.Remove(_explosions[e]);
            }
        }

        private void UpdateLaserBeams(GameTime gameTime)
        {
            for (var i = _laserBeams.Count - 1; i >= 0; i--)
            {
                _laserBeams[i].Update(gameTime);
                if (_laserBeams[i].Active == false) _laserBeams.RemoveAt(i);
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            var mousePosition = new Vector2(_currentMouseState.X, _currentMouseState.Y);

            if (_currentMouseState.LeftButton == ButtonState.Pressed)
            {
                var posDelta = mousePosition - _player.Position;
                posDelta.Normalize();
                posDelta *= _playerMoveSpeed;
                _player.Position += posDelta;
            }
            
            _player.Position.X += _currentGamePadState.ThumbSticks.Left.X * _playerMoveSpeed;
            _player.Position.Y += _currentGamePadState.ThumbSticks.Left.Y * _playerMoveSpeed;

            if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentGamePadState.DPad.Left == ButtonState.Pressed)
                _player.Position.X -= _playerMoveSpeed;
            if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentGamePadState.DPad.Right == ButtonState.Pressed)
                _player.Position.X += _playerMoveSpeed;
            if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentGamePadState.DPad.Up == ButtonState.Pressed)
                _player.Position.Y -= _playerMoveSpeed;
            if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentGamePadState.DPad.Down == ButtonState.Pressed)
                _player.Position.Y += _playerMoveSpeed;

            _player.Position.X = MathHelper.Clamp(_player.Position.X, _player.Width / 2, GraphicsDevice.Viewport.Width - _player.Width / 2);
            _player.Position.Y = MathHelper.Clamp(_player.Position.Y, _player.Height / 2, GraphicsDevice.Viewport.Height - _player.Height / 2);

            _player.Update(gameTime);

            if (_currentKeyboardState.IsKeyDown(Keys.Space) || _currentGamePadState.Buttons.X == ButtonState.Pressed)
                FireLaser(gameTime);

            if (_player.Health <= 0)
            {
                _player.Health = 100;
                _score = 0;
            }
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();

            _spriteBatch.Draw(_mainBackgroundTexture, Vector2.Zero, Color.White);
            _bgLayer1.Draw(_spriteBatch);
            _bgLayer2.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);

            foreach (var enemy in _enemies) enemy.Draw(_spriteBatch);

            foreach (var laser in _laserBeams) laser.Draw(_spriteBatch);

            foreach (var explosion in _explosions) explosion.Draw(_spriteBatch);

            _spriteBatch.DrawString(_font, "score: " + _score, new Vector2(
                    GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y),
                Color.White);

            _spriteBatch.DrawString(_font, "health: " + _player.Health, new Vector2(
                    GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30),
                Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}