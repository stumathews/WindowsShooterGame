using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsShooterGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private Player player;

        // For keyboard presses
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;

        // for gamepad states
        private GamePadState currentGamePadState;
        private GamePadState previousGamePadState;

        // for mouse states
        private MouseState currentMouseState;
        private MouseState previousMouseState;

        //movement speed for the player
        private float playerMoveSpeed;
        private Texture2D mainBackground;
        private ParallaxingBackground bgLayer1;
        private ParallaxingBackground bgLayer2;

        private Texture2D enemyTextture;
        private List<Enemy> enemies;

        // the rate at which the anemies will appear
        private TimeSpan enemySpawnTime;
        private TimeSpan previousSpawnTime;

        private Random random;

        private Texture2D laserTexture;
        private TimeSpan laserSpawnTime;
        private TimeSpan previousLaserSpawnTime;
        private List<Laser> laserBeams;

        private List<Explosion> explosions;
        private Texture2D explosionTexture;

        private SoundEffect laserSound;
        private SoundEffectInstance laserSoundEffectInstance;
        private SoundEffect explosionSound;
        private SoundEffectInstance explosionEffectInstance;

        private Song gameMusic;
        private SpriteFont font;
        private int score;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            score = 0;
            player = new Player();
            playerMoveSpeed = 8.0f;
            bgLayer1 = new ParallaxingBackground();
            bgLayer2 = new ParallaxingBackground();

            enemies = new List<Enemy>();

            previousSpawnTime = TimeSpan.Zero;
            enemySpawnTime = TimeSpan.FromSeconds(1.0f);
            random = new Random();

            laserBeams = new List<Laser>();
            const float SECONDS_IN_MINUTES = 60f;
            const float RATE_OF_FIRE = 200f;
            laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTES / RATE_OF_FIRE);
            previousLaserSpawnTime = TimeSpan.Zero;
            
            explosions = new List<Explosion>();
          


            base.Initialize();
        }

        protected void AddExplosion(Vector2 enemyPosition)
        {
            Animation explosionAnimation = new Animation();
            explosionAnimation.Initialize(explosionTexture,
                enemyPosition, 134,
                134,
                12, Color.White, 1.0f,
                true);

            Explosion explosion = new Explosion();
            explosion.Initialize(explosionAnimation, enemyPosition);
            explosions.Add(explosion);
            explosionEffectInstance.Play();
        }

        protected void FireLaser(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                AddLaser();

                laserSoundEffectInstance.Play();
            }
        }

        private void AddLaser()
        {
            Animation laserAnimation = new Animation();
            laserAnimation.Initialize(laserTexture, player.Position,
                46,16,1,Color.White, 1f, true);
            Laser laser = new Laser();

            var laserPosition = player.Position;
            laserPosition.X += 30;

            laser.Initialize(laserAnimation, laserPosition);
            laserBeams.Add(laser);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        
            // TODO: use this.Content to load your game content here
                
            var playerAnimation = new Animation();
            var playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);

            // Load in the texture into the animation object - this is a sprite strip texture...
            playerAnimation.Initialize(Content.Load<Texture2D>("Graphics\\shipAnimation"), Vector2.Zero, 115, 69, 8, Color.White, 1f, true);
            
            // Use the animation in the player
            player.Initialize(playerAnimation, playerPosition);

            bgLayer1.Initialize(Content, "Graphics/bgLayer1", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -1);
            bgLayer2.Initialize(Content, "Graphics/bgLayer2", GraphicsDevice.Viewport.Width,
                GraphicsDevice.Viewport.Height, -2);
            mainBackground = Content.Load<Texture2D>("Graphics/mainBackground");

            enemyTextture = Content.Load<Texture2D>("Graphics/mineAnimation");
            laserTexture = Content.Load<Texture2D>("Graphics/laser");
            explosionTexture = Content.Load<Texture2D>("Graphics/explosion");
            laserSound = Content.Load<SoundEffect>("Sound/laserFire");
            laserSoundEffectInstance = laserSound.CreateInstance();
            explosionSound = Content.Load<SoundEffect>("Sound/explosion");
            explosionEffectInstance = explosionSound.CreateInstance();
            font = Content.Load<SpriteFont>("Graphics/gameFont");
            gameMusic = Content.Load<Song>("Sound/gameMusic");
            MediaPlayer.Play(gameMusic);
        }

        private void AddEnemy()
        {
            Animation enemyAnimation = new Animation();

            enemyAnimation.Initialize(enemyTextture, Vector2.Zero, 47, 61, 8,  Color.White, 1f, true);
            Vector2 position = new Vector2(GraphicsDevice.Viewport.Width + enemyTextture.Width /2,
                random.Next(100, GraphicsDevice.Viewport.Height - 100));
            Enemy enemy = new Enemy();
            enemy.Initialize(enemyAnimation, position);
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
                previousSpawnTime = gameTime.TotalGameTime;
                AddEnemy();
            }

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                enemies[i].Update(gameTime);
                if (enemies[i].Active == false)
                {
                    score += enemies[i].Value;
                    enemies.RemoveAt(i);
                }
            }
        }

        private void UpdateCollision()
        {
            Rectangle playerRectangle;
            Rectangle enemyRectangle;
            Rectangle laserRectangle;

            playerRectangle = new Rectangle((int)player.Position.X,
                (int)player.Position.Y,
                player.Width,
                player.Height);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemyRectangle = new Rectangle((int)enemies[i].Position.X,
                    (int)enemies[i].Position.Y,
                    enemies[i].Width,
                    enemies[i].height);

                if (playerRectangle.Intersects(enemyRectangle))
                {
                    player.Health -= enemies[i].Damage;

                    enemies[i].Health = 0;

                    if (player.Health <= 0)
                        player.Active = false;

                    AddExplosion(enemies[i].Position);

                    enemies[i].Health = 0;

                }

                for(var l = 0; l < laserBeams.Count; l++)
                {
                    laserRectangle = new Rectangle(
                        (int) laserBeams[l].Position.X,
                        (int) laserBeams[l].Position.Y,
                        laserBeams[l].Width,
                        laserBeams[l].Height);

                    if (laserRectangle.Intersects(enemyRectangle))
                    {
                        enemies[i].Health = 0;
                        laserBeams[l].Active = false;
                        if (laserRectangle.Intersects(enemyRectangle))
                        {
                            AddExplosion(enemies[i].Position);

                            enemies[i].Health = 0;

                            laserBeams[l].Active = false;
                        }
                    }

                    
                }

                
            }

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            laserSoundEffectInstance.Dispose();
            explosionEffectInstance.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //save the previous state of our keyboard, gamepad and mouse so we can determine single key/button presses
            previousKeyboardState = currentKeyboardState;
            previousMouseState = currentMouseState;
            previousGamePadState = currentGamePadState;

            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            UpdatePlayer(gameTime);

            bgLayer1.Update(gameTime);
            bgLayer2.Update(gameTime);

            UpdateEnemies(gameTime);
            UpdateCollision();

            UpdateLaserBeams(gameTime);
            UpdateExplosions(gameTime);

            base.Update(gameTime);
        }

        private void UpdateExplosions(GameTime gameTime)
        {
            for(var e = explosions.Count-1; e >= 0;e--)
            {
                explosions[e].Update(gameTime);
                if(!explosions[e].Active)
                    explosions.Remove(explosions[e]);
            }
        }

        private void UpdateLaserBeams(GameTime gameTime)
        {
            for (int i = laserBeams.Count - 1; i >= 0; i--)
            {
                laserBeams[i].Update(gameTime);
                if (laserBeams[i].Active == false)
                {
                    laserBeams.RemoveAt(i);
                }
            }
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            // Get thhe mouse state - later we'll capur eof the mouse presses
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            if (currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 posDelta = mousePosition - player.Position;
                posDelta.Normalize();
                posDelta = posDelta * playerMoveSpeed;
                player.Position = player.Position + posDelta;
            }

            // get thumbstick controls
            player.Position.X += currentGamePadState.ThumbSticks.Left.X * playerMoveSpeed;
            player.Position.Y += currentGamePadState.ThumbSticks.Left.Y * playerMoveSpeed;
            
            // get keyboard controlds
            if (currentKeyboardState.IsKeyDown(Keys.Left) || currentGamePadState.DPad.Left == ButtonState.Pressed)
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right) || currentGamePadState.DPad.Right == ButtonState.Pressed)
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up) || currentGamePadState.DPad.Up == ButtonState.Pressed)
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down) || currentGamePadState.DPad.Down == ButtonState.Pressed)
            {
                player.Position.Y += playerMoveSpeed;
            }

            // make sure the player does not go out of bounds
            player.Position.X = MathHelper.Clamp(player.Position.X, player.Width / 2,
                GraphicsDevice.Viewport.Width - player.Width / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, player.Height / 2,
                GraphicsDevice.Viewport.Height - player.Height / 2);

            player.Update(gameTime);

            if (currentKeyboardState.IsKeyDown(Keys.Space) || currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                FireLaser(gameTime);
            }

            if(player.Health <= 0)
            {
                player.Health = 100;
                score = 0;
            }

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
            bgLayer1.Draw(spriteBatch);
            bgLayer2.Draw(spriteBatch);

            player.Draw(spriteBatch);

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            foreach (var l in laserBeams)
            {
                l.Draw(spriteBatch);
            }

            foreach (var explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }

            spriteBatch.DrawString(font, "score: " + score, new Vector2(
                GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y),
                Color.White);

            spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(
                    GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30),
                Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
