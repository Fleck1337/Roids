#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Flextensions;
#endregion
/*  TODO:
 *  ???
 *  BUGS:
 *  Firing projectile sometimes 'fires' 'somewhere'... This will be fun to track. POSSIBLE FIX IMPLEMENTED
*/
namespace Roids
{
    #region Game States
    enum GameState
    {
        Loading,
        StartScreen,
        Playing,
        GameOverScreen,
        HighscoreScreen
    }
    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameRoot : Microsoft.Xna.Framework.Game
    {
        #region Game Variables
        // Class Variables.
        GameState currGameState;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        bool debugToggle, debugPlayerHit, debugCheckingPixels;
        Random RNG;
        int lives, score, lastScore, level;
        float trailTimer, enemyTimer, spawnTime;

        // Objects.
        Player playerShip;
        List<Asteroid> asteroids;
        Asteroid tempAst;
        List<Projectile> playerProjectiles;
        List<Trail> trails;
        Saucer enemySaucer;

        // Highscore related vars.
        public static readonly string HighScoresFilename = "highscores.lst";
        bool enterName;
        string playerName;
        private Keys[] lastPressedKeys;

        // Volumes
        public float B_Volume = 0.1f, SE_Volume = 0.1f, Mu_Volume = 1f, Master_Volume = 0.5f;

        // Bloom Code.
        BloomComponent bloom;
        #endregion

        #region CONSTANTS
        public const int WIDTH = 960, HEIGHT = 720;
        private const int SCOREFORLIFE = 10000;
        private const float INVULNTIME = 2f;
        private const int SPAWNBORDER = 50;
        private const int MAXASTEROIDS = 12, MAXPLAYERPROJS = 4;
        private const int FRAGMENTSTOSPAWN = 3, STARTLIVES = 3, STARTLEVEL = 3;
        private const int STARTSPAWNTIME = 60, MINSPAWNTIME = 30;
        #endregion

        #region Constructor
        public GameRoot()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;

            // Instance the bloom component and add it to the components.
            bloom = new BloomComponent(this);
            Components.Add(bloom);
        }
        #endregion

        #region Init and Load
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            currGameState = GameState.Loading;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Load art and fonts.
            Art.Load(Content);
            Sound.Load(Content);
            ResetGame();
            // Set current gameState to Start Screen after loading assets from disk to avoid any stutter.
            currGameState = GameState.StartScreen;
        }
        #endregion

        #region Custom Functions
        // Create a list of Asteroids with a length given. Overwriting if necessary.
        public void CreateAsteroids(int number)
        {   
            // Limit the number before creating the Big asteroids, can still have many little ones, spawned elsewhere.
            if (number > MAXASTEROIDS)
                number = MAXASTEROIDS;

            asteroids = new List<Asteroid>();
            // Loop through the number of asteroids and add each one.
            for (int i = 0; i < number; i++)
            {
                // Randomise position. If in the center of the screen (as decided by SPAWNBORDER), try again.
                int xPos = RNG.Next(WIDTH), yPos = RNG.Next(HEIGHT);
                while (xPos > SPAWNBORDER && xPos < WIDTH - SPAWNBORDER && yPos > SPAWNBORDER && yPos < HEIGHT - SPAWNBORDER)
                {
                    xPos = RNG.Next(WIDTH);
                    yPos = RNG.Next(HEIGHT);
                }
                // Add the new asteroid to the list.
                asteroids.Add(new Asteroid(Art.Asteroids[RNG.Next(0, 8), 2], xPos, yPos, 3, RNG));
            }
        }

        public void ResetGame()
        {
            // Reset variables for the start of the game.
            RNG = new Random();
            playerShip = new Player(Art.PlayerShip, WIDTH / 2, HEIGHT / 2);
            playerProjectiles = new List<Projectile>();
            trails = new List<Trail>();
            // Does this help the disappearing trail?
            trails.Clear();
            CreateAsteroids(STARTLEVEL);
            debugToggle = false;
            debugPlayerHit = false;
            debugCheckingPixels = false;
            enemyTimer = 0;
            spawnTime = STARTSPAWNTIME;
            lives = STARTLIVES;
            score = 0;
            lastScore = 0;
            level = STARTLEVEL;
            Highscores.Init(HighScoresFilename);
            enterName = false;
            playerName = "";
            lastPressedKeys = new Keys[0];
        }

        private void KillPlayer()
        {
            // If debugging make background red to indicate hit.
            if (debugToggle)
                debugPlayerHit = true;
            // If not debugging, and we have outrun invuln, Remove Life etc.
            if (playerShip.TimeSinceSpawn > INVULNTIME && !debugToggle)
            {
                Sound.Hit.Play(SE_Volume * Master_Volume, 0f, 0f);
                lives--;
                playerShip = new Player(Art.PlayerShip, WIDTH / 2, HEIGHT / 2);
            }
            
        }

        private void PlayMusic()
        {
            if (MediaPlayer.State != MediaState.Playing)
            {
                // Play music
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = Mu_Volume * Master_Volume;
                MediaPlayer.Play(Sound.Music);
            }
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            #region Handle Inputs for All States
            // Must run so input information is updated per frame.
            Input.Update();
            // Allows the game to exit on controller Back or keyboard Escape at all times.
            if ((Input.IsButtonDown(Buttons.Back)) || (Input.IsKeyDown(Keys.Escape)))
                this.Exit();
            #endregion

            #region Update GameState StartScreen
            if (currGameState == GameState.StartScreen)
            {
                if (Input.WasButtonPressed(Buttons.Start) || Input.WasKeyPressed(Keys.Enter))
                {
                    Sound.Select.Play(B_Volume * Master_Volume, 0f, 0f);
                    currGameState = GameState.Playing;
                }
                if (Input.WasButtonPressed(Buttons.X) || Input.WasKeyPressed(Keys.X))
                {
                    Sound.Select.Play(B_Volume * Master_Volume, 0f, 0f);
                    currGameState = GameState.HighscoreScreen;
                }
            }
            #endregion

            #region Update GameState Playing
            if (currGameState == GameState.Playing)
            {
                #region Setup/Create Game Objects/Vars
                PlayMusic();
                debugPlayerHit = false;
                debugCheckingPixels = false;
                tempAst = asteroids[0];
                playerShip.Update(0.001f);
                #endregion

                #region Handle Saucer
                // Spawn Saucer on timer.
                if (enemyTimer >= spawnTime)
                {
                    enemyTimer = 0;
                    enemySaucer = new Saucer(Art.Saucer, RNG);
                    spawnTime -= 5;
                }
                if (spawnTime <= MINSPAWNTIME)
                    spawnTime = MINSPAWNTIME;
                // Do what the saucer needs to do.
                if (enemySaucer != null)
                {
                    enemySaucer.Update(playerShip.Position, RNG);
                    // Check Saucer and Player Collision.
                    if (Vector2.Distance(enemySaucer.Position, playerShip.Position) < enemySaucer.Radius + playerShip.Radius)
                    {
                        debugCheckingPixels = true;
                        #region if (PixelDetect)
                        if (PixelDetect(playerShip.shipTransform, playerShip.Radius * 2,
                                        playerShip.Radius * 2, playerShip.PlayerTexData,
                                        enemySaucer.saucerTransform, enemySaucer.Radius * 2,
                                        enemySaucer.Radius * 2, enemySaucer.SaucerTexData))
                        #endregion
                        {
                            KillPlayer();
                        }
                    }
                    // Check Saucer's Projectiles and Player Collision.
                    for (int i = 0; i < enemySaucer.EnemyProjectiles.Count(); i++)
                    {
                        if (Vector2.Distance(enemySaucer.EnemyProjectiles[i].Position, playerShip.Position) < playerShip.Radius + enemySaucer.EnemyProjectiles[i].Radius)
                        {
                            debugCheckingPixels = true;
                            #region if (PixelDetect)
                            if (PixelDetect(playerShip.shipTransform, playerShip.Radius * 2,
                                            playerShip.Radius * 2, playerShip.PlayerTexData,
                                            enemySaucer.EnemyProjectiles[i].Transform, enemySaucer.EnemyProjectiles[i].Radius * 2,
                                            enemySaucer.EnemyProjectiles[i].Radius * 2, enemySaucer.EnemyProjectiles[i].TexData))
                            #endregion
                            {
                                KillPlayer();
                                enemySaucer.EnemyProjectiles.RemoveAt(i);
                            }
                        }
                    }
                    // Check Saucer and Player's Projectiles Collision.
                    for (int i = 0; i < playerProjectiles.Count(); i++)
                    {
                        if (Vector2.Distance(enemySaucer.Position, playerProjectiles[i].Position) < enemySaucer.Radius)
                        {
                            score += 1000;
                            playerProjectiles.RemoveAt(i);
                            enemySaucer.Position.X = -100;
                            break;
                        }
                    }
                }
                #endregion

                #region Handle Inputs while Playing.
                // Handle Inputs during Gameplay.
#if (DEBUG)
                // Debug toggle on Y button press.
                if (Input.WasButtonPressed(Buttons.Y) || Input.WasKeyPressed(Keys.Y))
                    debugToggle = !debugToggle;
#endif
                // Spawn Trail
                if (Input.GetRightTrigger() > 0.1f || Input.IsKeyDown(Keys.W))
                {
                    float trailScale;
                    if (Input.IsKeyDown(Keys.W))
                        trailScale = 2f;
                    else
                        trailScale = 1f;
                    if (trailTimer >= 0.05f)
                    {
                        trailTimer = 0;
                        trails.Add(new Trail((int)playerShip.Position.X, (int)playerShip.Position.Y, new Vector2((float)Math.Cos(playerShip.Rotation), (float)Math.Sin(playerShip.Rotation)), 3f, trailScale + Input.GetRightTrigger()));
                    }
                }
                int projs = playerProjectiles.Count;
                // Shoot Projectile
                if ((Input.WasButtonPressed(Buttons.A) || Input.WasKeyPressed(Keys.Space)) && playerProjectiles.Count() < MAXPLAYERPROJS)
                {
                    Sound.Shoot.Play(SE_Volume * Master_Volume * 0.5f, (float)RNG.NextDouble(), 0f);
                    playerProjectiles.Add(new Projectile((int)playerShip.Position.X, (int)playerShip.Position.Y, new Vector2((float)Math.Cos(playerShip.Rotation), (float)Math.Sin(playerShip.Rotation)), 3f));
                    if (projs+1 != playerProjectiles.Count)
                        throw(new ApplicationException("Count has not incremented."));
                }
                // Spawn asteroids and saucer if debug.
                if ((Input.WasButtonPressed(Buttons.B) || Input.WasKeyPressed(Keys.B)) && debugToggle)
                {
                    CreateAsteroids(MAXASTEROIDS);
                    enemySaucer = new Saucer(Art.Saucer, RNG);
                }
                #endregion

                #region Loop through each asteroid and do what must be done...
                for (int i = 0; i < asteroids.Count(); i++)
                {
                    asteroids[i].Update();
                    #region if (distance is close enough to player)
                    if (Vector2.Distance(playerShip.Position, asteroids[i].Position) < playerShip.Radius + asteroids[i].Radius)
                    {
                        // Player may be hitting an asteroid!
                        debugCheckingPixels = true;
                        tempAst = asteroids[i];
                        // This should be the only call to PixelDetect, and as such is not my code.
                        #region if (PixelDetect)
                        if (PixelDetect(playerShip.shipTransform, playerShip.Radius * 2,
                                        playerShip.Radius * 2, playerShip.PlayerTexData,
                                        tempAst.roidTransform, tempAst.Radius * 2,
                                        tempAst.Radius * 2, tempAst.roidTexData))
                        #endregion
                        {
                            KillPlayer();
                        }
                    }
                    #endregion
                    #region Loop through playerProjectiles
                    for (int j = 0; j < playerProjectiles.Count(); j++)
                    {
                        if (Vector2.Distance(asteroids[i].Position, playerProjectiles[j].Position) < asteroids[i].Radius + playerProjectiles[j].Radius)
                        {
                            // We have shot an asteroid! 
                            // Kill it, the projectile, add to score and spawn fragments (if big enough).
                            Asteroid tempRoid = asteroids[i];
                            score += 60 / tempRoid.Size;
                            Sound.Explode.Play(SE_Volume * Master_Volume, (float)RNG.NextDouble(), 0f);
                            asteroids.RemoveAt(i);
                            playerProjectiles.RemoveAt(j);
                            if (tempRoid.Size > 1)
                            {
                                for (int k = 0; k < FRAGMENTSTOSPAWN; k++)
                                    asteroids.Add(new Asteroid(Art.Asteroids[RNG.Next(0, 8), tempRoid.Size  - 2], (int)tempRoid.Position.X, (int)tempRoid.Position.Y, tempRoid.Size - 1, RNG));
                            }
                            break;
                        }
                    }
                    #endregion
                    #region Loop through other asteroids
                    for (int k = i + 1; k < asteroids.Count(); k++)
                    {
                        if (Vector2.Distance(asteroids[i].Position, asteroids[k].Position) < asteroids[i].Radius + asteroids[k].Radius)
                        {
                            asteroids[i].HandleCollision(asteroids[k]);
                            asteroids[k].HandleCollision(asteroids[i]);
                        }
                    }
                    #endregion
                }
                #endregion

                #region Gameplay Stuff; Timers, Remove Projectiles, Handle Lives etc.
                // Timers
                enemyTimer += (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                trailTimer += (gameTime.ElapsedGameTime.Milliseconds / 1000f);

                // Remove projectiles after a while.
                for (int i = 0; i < playerProjectiles.Count(); i++)
                {
                    playerProjectiles[i].Update();
                    if (playerProjectiles[i].TimeSinceSpawn > playerProjectiles[i].Lifetime)
                        playerProjectiles.RemoveAt(i);
                }
                // Remove trail after a while.
                for (int i = 0; i < trails.Count(); i++)
                {
                    trails[i].Update();
                    if (trails[i].TimeSinceSpawn > trails[i].Lifetime)
                        trails.RemoveAt(i);
                }
                // Add a life every 10000(default) points.
                if (score - lastScore >= SCOREFORLIFE)
                {
                    lives++;
                    lastScore += SCOREFORLIFE;
                }
                // Game over when lives reach zero.
                if (lives <= 0)
                {
                    Sound.GameOver.Play(SE_Volume * Master_Volume, 0f, 0f);
                    currGameState = GameState.GameOverScreen;
                }
                // Once all asteroids are destroyed, start a new level.
                if (asteroids.Count() == 0)
                {
                    level++;
                    CreateAsteroids(level);
                }
                #endregion
            }
            #endregion

            #region Update GameState GameOver
            if (currGameState == GameState.GameOverScreen)
            {
                MediaPlayer.Stop();
                
                // If we are in the highscores allow us to enter name,
                if (Highscores.CheckHighScore(HighScoresFilename, score))
                {
                    #region We got Highscore
                    enterName = true;
                    Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();

                    // Check the last keys are no longer pressed.
                    foreach (Keys key in lastPressedKeys)
                    {
                        if (!pressedKeys.Contains(key) && AllowedKeys.IsAllowed(key) && playerName.Length < 15)
                            playerName += key.ToString();
                    }
                    // Save the currently pressed keys.
                    lastPressedKeys = pressedKeys;
                    // Allow the player to remove a character.
                    if (Input.WasKeyPressed(Keys.Back) && playerName.Length > 0)
                    {
                        playerName = playerName.Substring(0, playerName.Length - 1);
                    }
                    // Save Highscore and then display them.
                    if (Input.IsKeyDown(Keys.Enter) && playerName.Length > 0)
                    {
                        Sound.Select.Play(B_Volume * Master_Volume, 0f, 0f);
                        Highscores.SaveHighScore(HighScoresFilename, score, playerName);
                        enterName = false;
                        ResetGame();
                        currGameState = GameState.HighscoreScreen;
                    }
                    if (Input.IsKeyDown(Keys.Tab) || Input.IsButtonDown(Buttons.B))
                    {
                        enterName = false;
                        ResetGame();
                        currGameState = GameState.HighscoreScreen;
                    }
                    #endregion
                }
                else
                {
                    #region We no Highscore
                    ResetGame();
                    if (Input.WasButtonPressed(Buttons.X) || Input.WasKeyPressed(Keys.X))
                    {
                        Sound.Select.Play(B_Volume * Master_Volume, 0f, 0f);
                        currGameState = GameState.HighscoreScreen;
                    }
                    if (Input.WasButtonPressed(Buttons.B) || Input.WasKeyPressed(Keys.B))
                    {
                        Sound.Back.Play(B_Volume * Master_Volume, 0f, 0f);
                        currGameState = GameState.StartScreen;
                    }
#endregion
                }
            }
            #endregion

            #region Update GameState HighscoreScreen
            if (currGameState == GameState.HighscoreScreen)
            {
                if (Input.WasButtonPressed(Buttons.B) || Input.WasKeyPressed(Keys.B))
                {
                    Sound.Back.Play(B_Volume * Master_Volume, 0f, 0f);
                    currGameState = GameState.StartScreen;
                }
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Begin Bloom Post Effect.
            bloom.RedirectDraw();
            // Draw Blackground. Or Red if debugging and hit.
            GraphicsDevice.Clear(Color.Black);
            if (currGameState == GameState.Playing && debugToggle && debugPlayerHit)
                GraphicsDevice.Clear(Color.Red);

            #region Bloom Applied
            spriteBatch.Begin();

            #region Draw GameState Playing
            if (currGameState == GameState.Playing)
            {
                playerShip.Draw(spriteBatch);
                if (enemySaucer != null)
                {
                    enemySaucer.Draw(spriteBatch);
                    foreach (Projectile enemyProj in enemySaucer.EnemyProjectiles)
                    {
                        enemyProj.Draw(spriteBatch);
                    }
                }
                for (int i = 0; i < asteroids.Count(); i++)
                {
                    asteroids[i].Draw(spriteBatch);
                }
                foreach (Projectile playProj in playerProjectiles)
                {
                    playProj.Draw(spriteBatch);
                }
                foreach (Trail trail in trails)
                {
                    trail.Draw(spriteBatch);
                }
                
            }
            #endregion

            spriteBatch.End();
            #endregion

            base.Draw(gameTime);

            #region No Bloom Applied (UI/debug)
            spriteBatch.Begin();

            #region Draw UI GameState StartScreen
            if (currGameState == GameState.StartScreen)
            {
                string  titleText = "ROIDS",
                        subText0 = "Press Start to begin.",
                        subText1 = "Press X to view Highscores.";
                Vector2 titlePos = new Vector2(WIDTH / 2 - Art.TitleFont.MeasureString(titleText).X / 2, HEIGHT / 3);
                Vector2 subPos0 = new Vector2(WIDTH / 2 - Art.SubFont.MeasureString(subText0).X / 2, HEIGHT * 0.75f);
                Vector2 subPos1 = new Vector2(WIDTH / 2 - Art.SubFont.MeasureString(subText1).X / 2, (HEIGHT * 0.75f) + Art.SubFont.MeasureString("T").Y);
                spriteBatch.DrawString(Art.TitleFont, titleText, titlePos, Color.White);
                spriteBatch.DrawString(Art.SubFont, subText0, subPos0, Color.White);
                spriteBatch.DrawString(Art.SubFont, subText1, subPos1, Color.White);
            }
            #endregion

            #region Draw UI GameState Playing
            if (currGameState == GameState.Playing)
            {
                string scoreText = "Score: " + score;
                string warnText = "UFO Incoming!";
                Vector2 scorePos = new Vector2(WIDTH - Art.SubFont.MeasureString(scoreText).X, 0);
                Vector2 warnPos = new Vector2(WIDTH/ 2 - Art.SubFont.MeasureString(warnText).X / 2, 0);
                spriteBatch.DrawString(Art.SubFont, "Lives: " + lives, Vector2.Zero, Color.White);
                spriteBatch.DrawString(Art.SubFont, scoreText, scorePos, Color.White);
                if (spawnTime - enemyTimer < 5)
                {
                    spriteBatch.DrawString(Art.SubFont, warnText, warnPos, Color.White); 
                }
            }
            #endregion

            #region Draw UI GameState GameOver
            if (currGameState == GameState.GameOverScreen)
            {
                string  text = "Game Over!",
                        subText0 = "Press X to view Highscores.",
                        subText1 = "Press B to return to menu.";
                if (enterName)
                    text = "Enter Name:";
                spriteBatch.DrawString(Art.SubFont, text, new Vector2(WIDTH/2 - (Art.SubFont.MeasureString(text).X/2), HEIGHT/3), Color.White);
                if (enterName)
                    spriteBatch.DrawString(Art.SubFont, playerName, new Vector2(WIDTH / 2 - (Art.SubFont.MeasureString(playerName).X / 2), HEIGHT / 2), Color.White);
                if (!enterName)
                {
                    // SubText to display button prompts.
                    spriteBatch.DrawString(Art.SubFont, subText0, new Vector2(WIDTH / 2 - Art.SubFont.MeasureString(subText0).X / 2, (HEIGHT * 0.75f) + Art.SubFont.MeasureString("T").Y), Color.White);
                    spriteBatch.DrawString(Art.SubFont, subText1, new Vector2(WIDTH / 2 - (Art.SubFont.MeasureString(subText1).X / 2), HEIGHT - 10 - Art.SubFont.MeasureString(subText1).Y), Color.White);
                }

            }
            #endregion

            #region Draw UI GameState HighscoreScreen
            if (currGameState == GameState.HighscoreScreen)
            {
                Highscores.HighscoreData scores;
                scores = Highscores.LoadHighScores(HighScoresFilename);
                for (int i = 0; i < scores.Count; i++)
                {
                    spriteBatch.DrawString(Art.SubFont, scores.Names[i].ToString(), new Vector2(150, (Art.SubFont.MeasureString("T").Y + 20) * (i + 2)), Color.White);
                    spriteBatch.DrawString(Art.SubFont, scores.Scores[i].ToString(), new Vector2(WIDTH - 150 - Art.SubFont.MeasureString(scores.Scores[i].ToString()).X, (Art.SubFont.MeasureString("T").Y + 20) * (i + 2)), Color.White);
                }
                string titleText = "Highscores",
                        subText = "Press B to return to menu.";
                spriteBatch.DrawString(Art.TitleFont, titleText, new Vector2(WIDTH / 2 - (Art.TitleFont.MeasureString(titleText).X / 2), 10), Color.White);
                spriteBatch.DrawString(Art.SubFont, subText, new Vector2(WIDTH / 2 - (Art.SubFont.MeasureString(subText).X / 2), HEIGHT - 10 - Art.SubFont.MeasureString(subText).Y), Color.White);
            }
            #endregion

            #region Draw Debug UI
            if (debugToggle && currGameState == GameState.Playing)
            {
                spriteBatch.DrawString(Art.DebugFont,
                    "FPS: " +  (1 / (float)gameTime.ElapsedGameTime.TotalSeconds).ToString("N0") +
                    "\nPlayer Position: " + playerShip.Position +
                    "\nPlayer Velocity: " + playerShip.Velocity +
                    "\nPlayer Speed: " + playerShip.Velocity.Length() +
                    "\nNo. of Asteroids : " + asteroids.Count() +
                    "\nCheckPixels: " + debugCheckingPixels +
                    "\nPlayerHit: " + debugPlayerHit +
                    "\nSpawn UFO in: " + (spawnTime - enemyTimer) + "s." +
                    "\nTrails: " + trails.Count(),
                    new Vector2(0, Art.SubFont.MeasureString("T").Y), Color.White);
            }
            #endregion

            spriteBatch.End();
            #endregion
        }
        #endregion


        // NOT MY CODE - FROM SAMPLE!! 
        // I did however modify the pixel check from detecting any transparency, to mostly opaque.
        #region Per Pixel Check. This is not mine.
        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static bool PixelDetect(
                            Matrix transformA, int widthA, int heightA, Color[] dataA,
                            Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are opaque past a threshold,
                        if (colorA.A >= 0.99 && colorB.A >= 0.99)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }
        #endregion
    }
}
