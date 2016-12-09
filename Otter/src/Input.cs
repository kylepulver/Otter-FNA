using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Input {
        InputStates<Key> keyStates = new InputStates<Key>();
        InputStates<MouseButton> mouseButtonStates = new InputStates<MouseButton>();
        List<InputStates<int>> controllerButtonStates = new List<InputStates<int>>();

        public bool IsInitialized { get; private set; }

        public Game Game { get; internal set; }

        public List<Knob> Knobs = new List<Knob>();

        public static int GamepadsConnected = 16;

        public Input() {
            // FNA Text input extension
            TextInputEXT.TextInput += HandleTextInput;
            TextInputEXT.StartTextInput();

            // Create controllers
            for (int i = 0; i < GamepadsConnected; i++) {
                controllerButtonStates.Add(new InputStates<int>());
            }
        }

        void Initialize() {
            Game.Core.OnSDLKeyDown += HandleKeyDown;
            Game.Core.OnSDLKeyUp += HandleKeyUp;
            Game.Core.OnSDLMouseDown += HandleMouseButtonDown;
            Game.Core.OnSDLMouseUp += HandleMouseButtonUp;
            Game.Core.OnSDLMouseWheel += HandleMouseWheel;
            Game.Core.OnSDLMouseMotion += HandleMouseMotion;
            Game.Core.OnSDLControllerButtonDown += HandleControllerButtonDown;
            Game.Core.OnSDLControllerButtonUp += HandleControllerButtonUp;
            Game.Core.OnSDLControllerAxisMotion += HandleControllerAxisMotion;
        }

        internal void Update() {
            if (!IsInitialized) {
                Initialize();
                IsInitialized = true;
            }

            keyStates.Update();
            mouseButtonStates.Update();
            for (int i = 0; i < GamepadsConnected; i++) {
                controllerButtonStates.Add(new InputStates<int>());
            }

            foreach (var c in Knobs) {
                if (!c.IsUpdatedByInput) continue;
                c.UpdateInput(this);
            }
        }

        internal void PostUpdate() {
            keyStates.PostUpdate();
            mouseButtonStates.PostUpdate();

            MouseWheelDelta = 0;
        }

        public Key LastKey { get { return keyStates.Last; } }

        public bool IsKeyDown(Key key) {
            if (key == Key.Any) return keyStates.DownCurrentCount > 0;
            return keyStates.IsDownCurrent(key);
        }

        public bool IsKeyUp(Key key) {
            if (key == Key.Any) return true; // You really going to push ALL THE KEYS?
            return !IsKeyDown(key);
        }

        public bool IsKeyPressed(Key key) {
            if (key == Key.Any) return keyStates.DownCurrentCount > keyStates.DownPreviousCount;
            return IsKeyDown(key) && !keyStates.IsDownPrevious(key);
        }

        public bool IsKeyReleased(Key key) {
            if (key == Key.Any) return keyStates.DownCurrentCount < keyStates.DownPreviousCount;
            return IsKeyUp(key) && keyStates.IsDownPrevious(key);
        }

        public int MouseX;
        public int MouseY;

        public int MouseVirtualX;
        public int MouseVirtualY;

        public int MouseWheelDelta;

        public bool IsMouseLockedInWindow;

        public MouseButton LastMouseButton { get { return mouseButtonStates.Last; } }

        public bool IsMouseButtonDown(MouseButton mouseButton) {
            return mouseButtonStates.IsDownCurrent(mouseButton);
        }

        public bool IsMouseButtonUp(MouseButton mouseButton) {
            return !IsMouseButtonDown(mouseButton);
        }

        public bool IsMouseButtonPressed(MouseButton mouseButton) {
            return IsMouseButtonDown(mouseButton) && !mouseButtonStates.IsDownPrevious(mouseButton);
        }

        public bool IsMouseButtonReleased(MouseButton mouseButton) {
            return IsMouseButtonUp(mouseButton) && mouseButtonStates.IsDownPrevious(mouseButton);
        }

        public bool IsControllerButtonDown(int controllerId, int buttonId) {
            return controllerButtonStates[controllerId].IsDownCurrent(buttonId);
        }

        public bool IsControllerButtonUp(int controllerId, int buttonId) {
            return !IsControllerButtonDown(controllerId, buttonId);
        }

        public bool IsControllerButtonPressed(int controllerId, int buttonId) {
            return IsControllerButtonDown(controllerId, buttonId) && !controllerButtonStates[controllerId].IsDownPrevious(buttonId);
        }

        public bool IsControllerButtonReleased(int controllerId, int buttonId) {
            return IsControllerButtonUp(controllerId, buttonId) && controllerButtonStates[controllerId].IsDownPrevious(buttonId);
        }

        void HandleMouseButtonDown(MouseButton mouseButton) {
            mouseButtonStates.DownActive.Add(mouseButton);
        }

        void HandleMouseButtonUp(MouseButton mouseButton) {
            mouseButtonStates.UpActive.Add(mouseButton);
        }

        void HandleMouseWheel(int delta) {
            MouseWheelDelta = delta;
            Console.WriteLine("mouse wheel delta {0}", delta);
        }

        void HandleMouseMotion(int x, int y, int xRel, int yRel) {
            //Console.WriteLine("Mouse Pos {0} {1} Motion {2} {3}", x, y, xRel, yRel);
        }

        void HandleKeyDown(Key key) {
            keyStates.SetDown(key);
        }

        void HandleKeyUp(Key key) {
            keyStates.SetUp(key);
        }

        void HandleControllerButtonDown(int controllerId, int buttonId) {
            controllerButtonStates[controllerId].SetDown(buttonId);
        }

        void HandleControllerButtonUp(int controllerId, int buttonId) {
            controllerButtonStates[controllerId].SetUp(buttonId);
        }

        void HandleControllerAxisMotion(int controllerId, int axisId, float value) {

        }

        void HandleTextInput(char c) {
            if (c == (char)22) {
                TextInput += SDL2.SDL.SDL_GetClipboardText();
            }
            TextInput += c;
        }

        public string TextInput = "";
    }

    class InputStates<T> {
        public List<T> DownActive = new List<T>();
        public List<T> UpActive = new List<T>();
        public List<T> DownCurrent = new List<T>();
        public List<T> DownPrevious = new List<T>();
        public T Last;
        
        public void Update() {
            DownPrevious.Clear();
            DownPrevious.AddRange(DownCurrent);

            DownCurrent.Clear();
            DownCurrent.AddRange(DownActive);
        }

        public void PostUpdate() {
            foreach (var t in UpActive)
                if (DownActive.Contains(t))
                    DownActive.Remove(t);

            UpActive.Clear();
        }

        public void SetDown(T t) {
            DownActive.Add(t);
            Last = t;
        }

        public void SetUp(T t) {
            UpActive.Add(t);
        }

        public int DownCurrentCount { get { return DownCurrent.Count; } }
        public int DownPreviousCount { get { return DownPrevious.Count; } }

        public bool IsDownCurrent(T t) {
            return DownCurrent.Contains(t);
        }

        public bool IsDownActive(T t) {
            return DownActive.Contains(t);
        }

        public bool IsDownPrevious(T t) {
            return DownPrevious.Contains(t);
        }

        public bool IsUpActive(T t) {
            return UpActive.Contains(t);
        }
    }

    public enum MouseButton {
        Left = 1,
        Middle,
        Right,
        XButton1,
        XButton2,
        Any = 1000
    }

    public enum MouseWheelDirection {
        Up = 1,
        Down = -1,
        Any = 1000
    }

    public enum Key {
        None = 0,
        Back = 8,
        Tab = 9,
        Enter = 13,
        Pause = 19,
        CapsLock = 20,
        Kana = 21,
        Kanji = 25,
        Escape = 27,
        ImeConvert = 28,
        ImeNoConvert = 29,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Select = 41,
        Print = 42,
        Execute = 43,
        PrintScreen = 44,
        Insert = 45,
        Delete = 46,
        Help = 47,
        D0 = 48,
        D1 = 49,
        D2 = 50,
        D3 = 51,
        D4 = 52,
        D5 = 53,
        D6 = 54,
        D7 = 55,
        D8 = 56,
        D9 = 57,
        A = 65,
        B = 66,
        C = 67,
        D = 68,
        E = 69,
        F = 70,
        G = 71,
        H = 72,
        I = 73,
        J = 74,
        K = 75,
        L = 76,
        M = 77,
        N = 78,
        O = 79,
        P = 80,
        Q = 81,
        R = 82,
        S = 83,
        T = 84,
        U = 85,
        V = 86,
        W = 87,
        X = 88,
        Y = 89,
        Z = 90,
        LeftWindows = 91,
        RightWindows = 92,
        Apps = 93,
        Sleep = 95,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        Multiply = 106,
        Add = 107,
        Separator = 108,
        Subtract = 109,
        Decimal = 110,
        Divide = 111,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        F13 = 124,
        F14 = 125,
        F15 = 126,
        F16 = 127,
        F17 = 128,
        F18 = 129,
        F19 = 130,
        F20 = 131,
        F21 = 132,
        F22 = 133,
        F23 = 134,
        F24 = 135,
        NumLock = 144,
        Scroll = 145,
        LeftShift = 160,
        RightShift = 161,
        LeftControl = 162,
        RightControl = 163,
        LeftAlt = 164,
        RightAlt = 165,
        BrowserBack = 166,
        BrowserForward = 167,
        BrowserRefresh = 168,
        BrowserStop = 169,
        BrowserSearch = 170,
        BrowserFavorites = 171,
        BrowserHome = 172,
        VolumeMute = 173,
        VolumeDown = 174,
        VolumeUp = 175,
        MediaNextTrack = 176,
        MediaPreviousTrack = 177,
        MediaStop = 178,
        MediaPlayPause = 179,
        LaunchMail = 180,
        SelectMedia = 181,
        LaunchApplication1 = 182,
        LaunchApplication2 = 183,
        OemSemicolon = 186,
        OemPlus = 187,
        OemComma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        OemQuestion = 191,
        OemTilde = 192,
        ChatPadGreen = 202,
        ChatPadOrange = 203,
        OemOpenBrackets = 219,
        OemPipe = 220,
        OemCloseBrackets = 221,
        OemQuotes = 222,
        Oem8 = 223,
        OemBackslash = 226,
        ProcessKey = 229,
        OemCopy = 242,
        OemAuto = 243,
        OemEnlW = 244,
        Attn = 246,
        Crsel = 247,
        Exsel = 248,
        EraseEof = 249,
        Play = 250,
        Zoom = 251,
        Pa1 = 253,
        OemClear = 254,
        Any = 1000
    }

    [Flags]
    public enum Direction {
        None = 0,
        Up = 1 << 0,
        Right = 1 << 1,
        Down = 1 << 2,
        Left = 1 << 3,
        UpRight = Up | Right,
        UpLeft = Up | Left,
        DownRight = Down | Right,
        DownLeft = Down | Left
    }

    public enum GamepadAxis {
        X,
        Y,
        Z,
        R,
        U,
        V,
        PovX,
        PovY
    }
}
