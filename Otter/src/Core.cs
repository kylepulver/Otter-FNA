using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using static SDL2.SDL;

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

        internal event Action<Key> OnSDLKeyDown = delegate { };
        internal event Action<Key> OnSDLKeyUp = delegate { };
        internal event Action<MouseButton> OnSDLMouseDown = delegate { };
        internal event Action<MouseButton> OnSDLMouseUp = delegate { };
        internal event Action<int> OnSDLMouseWheel = delegate { };
        internal event Action<int, int, int, int> OnSDLMouseMotion = delegate { };
        internal event Action<int, int> OnSDLControllerButtonDown = delegate { };
        internal event Action<int, int> OnSDLControllerButtonUp = delegate { };
        internal event Action<int, int, float> OnSDLControllerAxisMotion = delegate { };

        SDL_EventFilter SDLEventFilter;

        internal Core(Game game) {
            Instance = this;

            this.game = game;

            SDLEventFilter = (data, ev) => {
                var e = (SDL_Event)System.Runtime.InteropServices.Marshal.PtrToStructure(ev, typeof(SDL_Event)); // o_o
                int keycode;
                Keys key;
                switch (e.type) {
                    case SDL_EventType.SDL_KEYDOWN:
                        if (e.key.repeat > 0) break;

                        keycode = (int)e.key.keysym.sym;
                        if (keyMap.ContainsKey(keycode))
                            key = keyMap[keycode];
                        else
                            key = Keys.None;
                        OnSDLKeyDown((Key)key);
                        break;
                    case SDL_EventType.SDL_KEYUP:
                        if (e.key.repeat > 0) break;

                        keycode = (int)e.key.keysym.sym;
                        if (keyMap.ContainsKey(keycode))
                            key = keyMap[keycode];
                        else
                            key = Keys.None;
                        OnSDLKeyUp((Key)key);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                        OnSDLMouseDown((MouseButton)e.button.button);
                        break;
                    case SDL_EventType.SDL_MOUSEBUTTONUP:
                        OnSDLMouseUp((MouseButton)e.button.button);
                        break;
                    case SDL_EventType.SDL_MOUSEWHEEL:
                        OnSDLMouseWheel(e.wheel.y);
                        break;
                    case SDL_EventType.SDL_MOUSEMOTION:
                        OnSDLMouseMotion(e.motion.x, e.motion.y, e.motion.xrel, e.motion.yrel);
                        break;
                    case SDL_EventType.SDL_JOYBUTTONDOWN:
                        //Console.WriteLine("controller {0} button {1}", e.jdevice.which, e.button.which);
                        break;
                    case SDL_EventType.SDL_CONTROLLERAXISMOTION:
                        OnSDLControllerAxisMotion(e.cdevice.which, e.caxis.which, e.caxis.axis);
                        //Console.WriteLine("YO {0} id {1} axis {2}", e.caxis.axisValue, e.caxis.which, e.caxis.axis);
                        break;
                    case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                        OnSDLControllerButtonDown(e.cdevice.which, e.button.button);
                        Console.WriteLine("controller {0} button {1}", e.cdevice.which, (SDL_GameControllerButton)e.button.which);
                        break;
                    case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                        OnSDLControllerButtonUp(e.cdevice.which, e.button.button);
                        break;
                    case SDL_EventType.SDL_CONTROLLERDEVICEADDED:

                        break;
                }
                return 0;
            };

            IsFixedTimeStep = false;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = game.Width;
            graphics.PreferredBackBufferHeight = game.Height;
            graphics.SynchronizeWithVerticalRetrace = false;

            Content.RootDirectory = "Content";

            OnGraphicsDeviceReady(GraphicsDevice);
            IsReady = true;
            OnGraphicsDeviceReady = delegate { }; // Clear it after calling it.
            Console.WriteLine("Graphics Device Ready");
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // SDL events for better input
            SDL_AddEventWatch(SDLEventFilter, IntPtr.Zero);

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

            game.Initialize();
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

            // Draw everything to the game surface
            GraphicsDevice.SetRenderTarget(game.Surface.Target);
            game.Draw.Clear(game.Color);
            game.Draw.Begin();
            game.Render();
            game.Draw.End();

            // Draw game surface to window
            GraphicsDevice.SetRenderTarget(null);
            game.Draw.Clear(Color.None);
            SpriteBatch.Begin(); // special case here for drawing game surface
            SpriteBatch.Draw(game.Surface.Target, new Vector2(0, 0), Color.White.ToXnaColor());
            SpriteBatch.End();

            stopwatch.Stop();
            game.RenderTime = stopwatch.Elapsed.Ticks / (float)TimeSpan.TicksPerSecond;
        }

        private static Dictionary<int, Keys> keyMap = new Dictionary<int, Keys>() {
            { (int) SDL_Keycode.SDLK_a,                 Keys.A },
            { (int) SDL_Keycode.SDLK_b,                 Keys.B },
            { (int) SDL_Keycode.SDLK_c,                 Keys.C },
            { (int) SDL_Keycode.SDLK_d,                 Keys.D },
            { (int) SDL_Keycode.SDLK_e,                 Keys.E },
            { (int) SDL_Keycode.SDLK_f,                 Keys.F },
            { (int) SDL_Keycode.SDLK_g,                 Keys.G },
            { (int) SDL_Keycode.SDLK_h,                 Keys.H },
            { (int) SDL_Keycode.SDLK_i,                 Keys.I },
            { (int) SDL_Keycode.SDLK_j,                 Keys.J },
            { (int) SDL_Keycode.SDLK_k,                 Keys.K },
            { (int) SDL_Keycode.SDLK_l,                 Keys.L },
            { (int) SDL_Keycode.SDLK_m,                 Keys.M },
            { (int) SDL_Keycode.SDLK_n,                 Keys.N },
            { (int) SDL_Keycode.SDLK_o,                 Keys.O },
            { (int) SDL_Keycode.SDLK_p,                 Keys.P },
            { (int) SDL_Keycode.SDLK_q,                 Keys.Q },
            { (int) SDL_Keycode.SDLK_r,                 Keys.R },
            { (int) SDL_Keycode.SDLK_s,                 Keys.S },
            { (int) SDL_Keycode.SDLK_t,                 Keys.T },
            { (int) SDL_Keycode.SDLK_u,                 Keys.U },
            { (int) SDL_Keycode.SDLK_v,                 Keys.V },
            { (int) SDL_Keycode.SDLK_w,                 Keys.W },
            { (int) SDL_Keycode.SDLK_x,                 Keys.X },
            { (int) SDL_Keycode.SDLK_y,                 Keys.Y },
            { (int) SDL_Keycode.SDLK_z,                 Keys.Z },
            { (int) SDL_Keycode.SDLK_0,                 Keys.D0 },
            { (int) SDL_Keycode.SDLK_1,                 Keys.D1 },
            { (int) SDL_Keycode.SDLK_2,                 Keys.D2 },
            { (int) SDL_Keycode.SDLK_3,                 Keys.D3 },
            { (int) SDL_Keycode.SDLK_4,                 Keys.D4 },
            { (int) SDL_Keycode.SDLK_5,                 Keys.D5 },
            { (int) SDL_Keycode.SDLK_6,                 Keys.D6 },
            { (int) SDL_Keycode.SDLK_7,                 Keys.D7 },
            { (int) SDL_Keycode.SDLK_8,                 Keys.D8 },
            { (int) SDL_Keycode.SDLK_9,                 Keys.D9 },
            { (int) SDL_Keycode.SDLK_KP_0,              Keys.NumPad0 },
            { (int) SDL_Keycode.SDLK_KP_1,              Keys.NumPad1 },
            { (int) SDL_Keycode.SDLK_KP_2,              Keys.NumPad2 },
            { (int) SDL_Keycode.SDLK_KP_3,              Keys.NumPad3 },
            { (int) SDL_Keycode.SDLK_KP_4,              Keys.NumPad4 },
            { (int) SDL_Keycode.SDLK_KP_5,              Keys.NumPad5 },
            { (int) SDL_Keycode.SDLK_KP_6,              Keys.NumPad6 },
            { (int) SDL_Keycode.SDLK_KP_7,              Keys.NumPad7 },
            { (int) SDL_Keycode.SDLK_KP_8,              Keys.NumPad8 },
            { (int) SDL_Keycode.SDLK_KP_9,              Keys.NumPad9 },
            { (int) SDL_Keycode.SDLK_KP_CLEAR,          Keys.OemClear },
            { (int) SDL_Keycode.SDLK_KP_DECIMAL,        Keys.Decimal },
            { (int) SDL_Keycode.SDLK_KP_DIVIDE,         Keys.Divide },
            { (int) SDL_Keycode.SDLK_KP_ENTER,          Keys.Enter },
            { (int) SDL_Keycode.SDLK_KP_MINUS,          Keys.Subtract },
            { (int) SDL_Keycode.SDLK_KP_MULTIPLY,       Keys.Multiply },
            { (int) SDL_Keycode.SDLK_KP_PERIOD,         Keys.OemPeriod },
            { (int) SDL_Keycode.SDLK_KP_PLUS,           Keys.Add },
            { (int) SDL_Keycode.SDLK_F1,                Keys.F1 },
            { (int) SDL_Keycode.SDLK_F2,                Keys.F2 },
            { (int) SDL_Keycode.SDLK_F3,                Keys.F3 },
            { (int) SDL_Keycode.SDLK_F4,                Keys.F4 },
            { (int) SDL_Keycode.SDLK_F5,                Keys.F5 },
            { (int) SDL_Keycode.SDLK_F6,                Keys.F6 },
            { (int) SDL_Keycode.SDLK_F7,                Keys.F7 },
            { (int) SDL_Keycode.SDLK_F8,                Keys.F8 },
            { (int) SDL_Keycode.SDLK_F9,                Keys.F9 },
            { (int) SDL_Keycode.SDLK_F10,               Keys.F10 },
            { (int) SDL_Keycode.SDLK_F11,               Keys.F11 },
            { (int) SDL_Keycode.SDLK_F12,               Keys.F12 },
            { (int) SDL_Keycode.SDLK_F13,               Keys.F13 },
            { (int) SDL_Keycode.SDLK_F14,               Keys.F14 },
            { (int) SDL_Keycode.SDLK_F15,               Keys.F15 },
            { (int) SDL_Keycode.SDLK_F16,               Keys.F16 },
            { (int) SDL_Keycode.SDLK_F17,               Keys.F17 },
            { (int) SDL_Keycode.SDLK_F18,               Keys.F18 },
            { (int) SDL_Keycode.SDLK_F19,               Keys.F19 },
            { (int) SDL_Keycode.SDLK_F20,               Keys.F20 },
            { (int) SDL_Keycode.SDLK_F21,               Keys.F21 },
            { (int) SDL_Keycode.SDLK_F22,               Keys.F22 },
            { (int) SDL_Keycode.SDLK_F23,               Keys.F23 },
            { (int) SDL_Keycode.SDLK_F24,               Keys.F24 },
            { (int) SDL_Keycode.SDLK_SPACE,             Keys.Space },
            { (int) SDL_Keycode.SDLK_UP,                Keys.Up },
            { (int) SDL_Keycode.SDLK_DOWN,              Keys.Down },
            { (int) SDL_Keycode.SDLK_LEFT,              Keys.Left },
            { (int) SDL_Keycode.SDLK_RIGHT,             Keys.Right },
            { (int) SDL_Keycode.SDLK_LALT,              Keys.LeftAlt },
            { (int) SDL_Keycode.SDLK_RALT,              Keys.RightAlt },
            { (int) SDL_Keycode.SDLK_LCTRL,             Keys.LeftControl },
            { (int) SDL_Keycode.SDLK_RCTRL,             Keys.RightControl },
            { (int) SDL_Keycode.SDLK_LGUI,              Keys.LeftWindows },
            { (int) SDL_Keycode.SDLK_RGUI,              Keys.RightWindows },
            { (int) SDL_Keycode.SDLK_LSHIFT,            Keys.LeftShift },
            { (int) SDL_Keycode.SDLK_RSHIFT,            Keys.RightShift },
            { (int) SDL_Keycode.SDLK_APPLICATION,       Keys.Apps },
            { (int) SDL_Keycode.SDLK_SLASH,             Keys.OemQuestion },
            { (int) SDL_Keycode.SDLK_BACKSLASH,         Keys.OemBackslash },
            { (int) SDL_Keycode.SDLK_LEFTBRACKET,       Keys.OemOpenBrackets },
            { (int) SDL_Keycode.SDLK_RIGHTBRACKET,      Keys.OemCloseBrackets },
            { (int) SDL_Keycode.SDLK_CAPSLOCK,          Keys.CapsLock },
            { (int) SDL_Keycode.SDLK_COMMA,             Keys.OemComma },
            { (int) SDL_Keycode.SDLK_DELETE,            Keys.Delete },
            { (int) SDL_Keycode.SDLK_END,               Keys.End },
            { (int) SDL_Keycode.SDLK_BACKSPACE,         Keys.Back },
            { (int) SDL_Keycode.SDLK_RETURN,            Keys.Enter },
            { (int) SDL_Keycode.SDLK_ESCAPE,            Keys.Escape },
            { (int) SDL_Keycode.SDLK_HOME,              Keys.Home },
            { (int) SDL_Keycode.SDLK_INSERT,            Keys.Insert },
            { (int) SDL_Keycode.SDLK_MINUS,             Keys.OemMinus },
            { (int) SDL_Keycode.SDLK_NUMLOCKCLEAR,      Keys.NumLock },
            { (int) SDL_Keycode.SDLK_PAGEUP,            Keys.PageUp },
            { (int) SDL_Keycode.SDLK_PAGEDOWN,          Keys.PageDown },
            { (int) SDL_Keycode.SDLK_PAUSE,             Keys.Pause },
            { (int) SDL_Keycode.SDLK_PERIOD,            Keys.OemPeriod },
            { (int) SDL_Keycode.SDLK_EQUALS,            Keys.OemPlus },
            { (int) SDL_Keycode.SDLK_PRINTSCREEN,       Keys.PrintScreen },
            { (int) SDL_Keycode.SDLK_QUOTE,             Keys.OemQuotes },
            { (int) SDL_Keycode.SDLK_SCROLLLOCK,        Keys.Scroll },
            { (int) SDL_Keycode.SDLK_SEMICOLON,         Keys.OemSemicolon },
            { (int) SDL_Keycode.SDLK_SLEEP,             Keys.Sleep },
            { (int) SDL_Keycode.SDLK_TAB,               Keys.Tab },
            { (int) SDL_Keycode.SDLK_BACKQUOTE,         Keys.OemTilde },
            { '²' /* FIXME: AZERTY SDL2? -flibit */,    Keys.OemTilde },
            { 'é' /* FIXME: BEPO SDL2? -flibit */,      Keys.None },
            { '|' /* FIXME: Norwegian SDL2? -flibit */, Keys.OemPipe },
            { '+' /* FIXME: Norwegian SDL2? -flibit */, Keys.OemPlus },
            { 'ø' /* FIXME: Norwegian SDL2? -flibit */, Keys.OemSemicolon },
            { 'æ' /* FIXME: Norwegian SDL2? -flibit */, Keys.OemQuotes },
            { (int) SDL_Keycode.SDLK_UNKNOWN,           Keys.None }
        };
    }
}
