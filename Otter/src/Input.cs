using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Input {

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        MouseState currentMouseState;
        MouseState previousMouseState;

        Vector2 currentMousePosition;
        Vector2 previousMousePosition;

        bool isLocked;
        public bool IsMouseLockedInWindow;

        public Game Game { get; internal set; }

        Dictionary<MouseButton, bool> mouseButtonsDown = new Dictionary<MouseButton, bool>();
        Dictionary<MouseButton, bool> previousMouseButtonsDown = new Dictionary<MouseButton, bool>();

        public Input() {
            Util.EnumValues<MouseButton>().Each(mb => {
                mouseButtonsDown.Add(mb, false);
                previousMouseButtonsDown.Add(mb, false);
            });
        }

        public void Update() {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            previousMousePosition = currentMousePosition;
            currentMousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            MouseWheelPreviousPosition = MouseWheelPosition;
            MouseWheelPosition = currentMouseState.ScrollWheelValue;

            if (IsMouseLockedInWindow) {
                if (!isLocked) {
                    isLocked = true;

                    MouseVirtualX = MouseWindowX;
                    MouseVirtualY = MouseWindowY;

                    Mouse.SetPosition(Game.HalfWidth, Game.HalfHeight);
                }
                else {
                    MouseVirtualPreviousX = MouseVirtualX;
                    MouseVirtualPreviousY = MouseVirtualY;

                    MouseVirtualX += (int)currentMousePosition.X - Game.HalfWidth;
                    MouseVirtualY += (int)currentMousePosition.Y - Game.HalfHeight;

                    Mouse.SetPosition(Game.HalfWidth, Game.HalfHeight);
                }
            }
            else {
                if (isLocked) isLocked = false;

                MouseWindowPreviousX = MouseWindowX;
                MouseWindowPreviousY = MouseWindowY;

                MouseWindowX = (int)currentMousePosition.X;
                MouseWindowY = (int)currentMousePosition.Y;
            }

            foreach (var kv in mouseButtonsDown) {
                previousMouseButtonsDown[kv.Key] = mouseButtonsDown[kv.Key];
            }
            
            Util.EnumValues<MouseButton>().Each(mb => {
                if (ConvertMouseButton(currentMouseState, mb) == ButtonState.Pressed) {
                    mouseButtonsDown[mb] = true;
                }
                if (ConvertMouseButton(currentMouseState, mb) == ButtonState.Released) {
                    mouseButtonsDown[mb] = false;
                }
            });
        }

        bool KeyState(Key key) {
            return currentKeyboardState.IsKeyDown((Keys)key);
        }

        bool PreviousKeyState(Key key) {
            return previousKeyboardState.IsKeyDown((Keys)key);
        }

        public bool IsKeyPressed(Key key) {
            return KeyState(key) && !PreviousKeyState(key);
        }

        public bool IsKeyDown(Key key) {
            return KeyState(key);
        }

        public bool IsKeyUp(Key key) {
            return !KeyState(key);
        }

        public bool IsKeyReleased(Key key) {
            return !KeyState(key) && PreviousKeyState(key);
        }

        public int MouseVirtualX;

        public int MouseVirtualY;

        public int MouseWindowX;

        public int MouseWindowY;

        public int MouseVirtualPreviousX;

        public int MouseVirtualPreviousY;

        public int MouseWindowPreviousX;

        public int MouseWindowPreviousY;

        public int MouseWheelPosition;

        public int MouseWheelPreviousPosition;

        public int MouseX {
            get {
                if (IsMouseLockedInWindow)
                    return MouseVirtualX;

                return MouseWindowX;
            }
            set {
                currentMousePosition.X = value;
                Mouse.SetPosition((int)currentMousePosition.X, (int)currentMousePosition.Y);
            }
        }
        public int MouseY {
            get {
                if (IsMouseLockedInWindow)
                    return MouseVirtualY;

                return MouseWindowY;
            }
            set {
                currentMousePosition.Y = value;
                Mouse.SetPosition((int)currentMousePosition.X, (int)currentMousePosition.Y);
            }
        }

        public int MousePreviousX {
            get {
                if (IsMouseLockedInWindow)
                    return MouseVirtualPreviousX;

                return MouseWindowPreviousX;
            }
        }

        public int MousePreviousY {
            get {
                if (IsMouseLockedInWindow)
                    return MouseVirtualPreviousY;

                return MouseWindowPreviousY;
            }
        }

        public int MouseDeltaX {
            get { return MouseX - MousePreviousX; }
        }

        public int MouseDeltaY {
            get { return MouseY - MousePreviousY; }
        }

        public int MouseWheelDelta {
            get { return currentMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue; }
        }

        public bool IsMouseButtonPressed(MouseButton mouseButton) {
            return !previousMouseButtonsDown[mouseButton] && mouseButtonsDown[mouseButton];
        }

        public bool IsMouseButtonReleased(MouseButton mouseButton) {
            return previousMouseButtonsDown[mouseButton] && !mouseButtonsDown[mouseButton];
        }

        public bool IsMouseButtonDown(MouseButton mouseButton) {
            return mouseButtonsDown[mouseButton];
        }

        public bool IsMouseButtonUp(MouseButton mouseButton) {
            return !mouseButtonsDown[mouseButton];
        }

        public bool IsGamePadButtonPressed(int id, int buttonId) {
            return true;
        }

        public bool IsGamePadButtonReleased(int id, int buttonId) {
            return true;
        }

        public bool IsGamePadButtonDown(int id, int buttonId) {
            return true;
        }

        public bool IsGamePadButtonUp(int id, int buttonId) {
            return true;
        }

        public Vector2 GetGamePadAxis(int id) {
            return Vector2.Zero;
        }

        ButtonState ConvertMouseButton(MouseState ms, MouseButton mb) {
            switch (mb) {
                case MouseButton.Left: return ms.LeftButton;
                case MouseButton.Right: return ms.RightButton;
                case MouseButton.Middle: return ms.MiddleButton;
                case MouseButton.XButton1: return ms.XButton1;
                case MouseButton.XButton2: return ms.XButton2;
                default: return ms.LeftButton;
            }
        }
    }

    public enum MouseButton {
        Left,
        Right,
        Middle,
        XButton1,
        XButton2
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
    }
}
