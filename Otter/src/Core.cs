using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Otter {
    class Core : Microsoft.Xna.Framework.Game {
        GraphicsDeviceManager graphics;
        Game game;

        internal SpriteBatch SpriteBatch;
        internal bool IsReady { get; private set; }
        internal static Core Instance;

        internal Action<GraphicsDevice> OnGraphicsDeviceReady = delegate { };

        internal Texture2D WhitePixel;

        Stopwatch stopwatch = new Stopwatch();

        public Core(Game game) {
            Instance = this;

            this.game = game;

            IsFixedTimeStep = false;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = game.Width;
            graphics.PreferredBackBufferHeight = game.Height;
            graphics.SynchronizeWithVerticalRetrace = false;
            
            this.Content.RootDirectory = "Content";
        }

        

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            game.Initialize();
            WhitePixel = new Texture2D(GraphicsDevice, 1, 1);
            WhitePixel.SetData(new Microsoft.Xna.Framework.Color[] { Color.White.ToXnaColor() });

            Environment.SetEnvironmentVariable("FNA_GAMEPAD_NUM_GAMEPADS", "16");
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            OnGraphicsDeviceReady(GraphicsDevice);
            OnGraphicsDeviceReady = delegate { }; // Clear it after calling it.
            
            IsReady = true;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent() {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            Window.Title = game.Title + game.TitleExtra;
            IsMouseVisible = game.IsMouseVisible;

            game.RealDeltaTime = gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond;
            stopwatch.Restart();
            game.Run();
            stopwatch.Stop();
            game.UpdateTime = stopwatch.Elapsed.Ticks / (float)TimeSpan.TicksPerSecond;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            stopwatch.Restart();

            GraphicsDevice.SetRenderTarget(game.Surface.Target);
            game.Draw.Clear(game.Color);
            game.Draw.Begin();
            game.Render();
            game.Draw.End();

            GraphicsDevice.SetRenderTarget(null);
            game.Draw.Clear(Color.None);
            SpriteBatch.Begin(); // special case here for drawing game surface
            SpriteBatch.Draw((Texture2D)game.Surface.Target, new Vector2(0, 0), Color.White.ToXnaColor());
            SpriteBatch.End();

            stopwatch.Stop();
            game.RenderTime = stopwatch.Elapsed.Ticks / (float)TimeSpan.TicksPerSecond;
        }
    }
}
