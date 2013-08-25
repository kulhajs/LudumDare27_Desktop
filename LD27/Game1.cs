#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
#endregion

namespace LD27
{
    enum GameState { 
        StartMenu,
        Running,
        LevelCompleted,
        LevelFailed,
        Completed
    };

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font10;

        LevelHandler levelHandler;
        Player player;
        Lava lava;

        KeyboardState currentKeyboardState;
        KeyboardState oldKeyboardState;


        GameState currentGameState = GameState.StartMenu;

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 480;
            graphics.PreferredBackBufferHeight = 800;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well
        /// </summary>
        protected override void Initialize()
        {
            levelHandler = new LevelHandler(Content);
            levelHandler.InitializeLevel();
            player = new Player()
            {
                CurrentLevel = levelHandler.CurrentLevel
            };
            lava = new Lava();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            player.LoadContent(Content);
            lava.LoadContent(Content);

            font10 = Content.Load<SpriteFont>("Fonts/font_10");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (currentGameState == GameState.StartMenu)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Space) && 
                    !oldKeyboardState.IsKeyDown(Keys.Space))
                {
                    currentGameState = GameState.Running;
                }
            }
            else if (currentGameState == GameState.LevelCompleted && 
                     levelHandler.CurrentLevelId == 5)
            {
                currentGameState = GameState.Completed;
            }
            else if (currentGameState == GameState.LevelCompleted || 
                     currentGameState == GameState.LevelFailed && 
                     currentKeyboardState.IsKeyDown(Keys.Space) && 
                     !oldKeyboardState.IsKeyDown(Keys.Space))
            {
                if (currentGameState == GameState.LevelCompleted) levelHandler.CurrentLevelId += 1;
                levelHandler.InitializeLevel();
                lava.Initialize();
                player.Initialize();
                player.CurrentLevel = levelHandler.CurrentLevel;
                currentGameState = GameState.StartMenu;
            }

            if (currentGameState == GameState.Running)
            {
                if (player.Y - lava.TopPosition.Y > 100)
                {
                    currentGameState = GameState.LevelFailed;
                }

                if (player.Y > -110) player.Update(gameTime, currentKeyboardState, oldKeyboardState); else currentGameState = GameState.LevelCompleted;
            }

            if (currentGameState != GameState.StartMenu)
            {
                lava.Update(gameTime);
            }

            oldKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            levelHandler.DrawLevel(this.spriteBatch);
            player.Draw(this.spriteBatch);
            lava.Draw(this.spriteBatch);

            if (currentGameState == GameState.StartMenu)
            {
                spriteBatch.DrawString(font10, "PRESS SPACE TO START", new Vector2(85, 400), Color.Black);
            }
            else if (currentGameState == GameState.LevelCompleted)
            {
                spriteBatch.DrawString(font10, "PRESS SPACE FOR NEXT LEVEL", new Vector2(50, 400), Color.Black);
            }
            else if (currentGameState == GameState.LevelFailed)
            {
                spriteBatch.DrawString(font10, "PRESS SPACE TO TRY AGAIN", new Vector2(60, 400), Color.Black);
            }
            else if (currentGameState == GameState.Completed)
            {
                spriteBatch.DrawString(font10, "YOU'RE VICTORIOUS!", new Vector2(85, 400), Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
