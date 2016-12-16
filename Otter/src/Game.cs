using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Otter {
    public class Game {
        internal Core Core;

        public int Width;
        public int Height;
        public string Title;

        internal string TitleExtra;

        public int HalfWidth { get { return Width / 2; } }
        public int HalfHeight { get { return Height / 2; } }

        public Vector2 Center { get { return new Vector2(HalfWidth, HalfHeight); } }

        public static Game Instance { get; private set; }

        public Coroutine Coroutine { get; private set; }

        public Tweener Tweener { get; private set; }

        public Input Input { get; private set; }

        public Draw Draw { get; private set; }

        public Process Process;

        public int TargetFramesPerSecond = 60;

        public bool ShowPerformanceInTitle;

        public bool IsMouseVisible;

        public bool HasUpdatedOnce { get; internal set; }

        public float RealDeltaTime { get; internal set; }
        public float DeltaTime {
            get {
                if (IsFixedTimeStep) return 1;
                else return RealDeltaTime;
            }
        }

        public float UpdateTime { get; internal set; }
        public float RenderTime { get; internal set; }
        public float FramesPerSecond { get; internal set; }
        public float FrameTime { get; internal set; }

        public event Action OnPreUpdate = delegate { };
        public event Action OnPostUpdate = delegate { };
        public event Action OnUpdate = delegate { };
        public event Action OnRender = delegate { };
        public event Action OnInit = delegate { };

        public int ElaspedFrames { get; private set; }

        public bool IsFixedTimeStep;

        public Color Color;

        public Surface Surface;

        public List<Surface> Surfaces = new List<Surface>();

        public List<Scene> Scenes = new List<Scene>();

        internal Scene BufferedScene;

        internal bool IsInitialized;

        float elapsedTime;

        PerformanceTracker PerformanceTracker = new PerformanceTracker();

        public Scene Scene {
            set {
                Scenes.Clear();
                Scenes.Add(value);
            }
            get {
                if (Scenes.Count == 0) return null;
                return Scenes.Last();
            }
        }

        public Game(int width, int height, string title) {
            Instance = this;

            Coroutine = new Coroutine();
            Tweener = new Tweener();
            Input = new Input() { Game = this };
            Draw = new Draw();

            Title = title;
            Width = width;
            Height = height;

            Process = Process.GetCurrentProcess();

            Core = new Core(this);
        }

        public void SetWindowScale(float scale) {
            Core.GraphicsManager.PreferredBackBufferWidth = (int)(Width * scale);
            Core.GraphicsManager.PreferredBackBufferHeight = (int)(Height * scale);

            if (Surface != null)
                UpdateWindow();
        }

        void UpdateWindow() {
            Core.GraphicsManager.ApplyChanges();

            Surface.ScaledWidth = Core.Window.ClientBounds.Width;
            Surface.ScaledHeight = Core.Window.ClientBounds.Height;
            Surface.CenterOrigin();
            Surface.X = Core.Window.ClientBounds.Width / 2;
            Surface.Y = Core.Window.ClientBounds.Height / 2;
        }

        public void SwitchScene(Scene scene) {
            BufferedScene = scene;
        }

        public void Start<SceneType>() where SceneType : Scene {
            var scene = Activator.CreateInstance<SceneType>();
            SwitchScene(scene);
            Core.Run();
        }

        public void Start(Scene scene) {
            SwitchScene(scene);
            Core.Run();
        }

        public void Start() {
            Core.Run();
        }

        public float CameraX {
            get { return Surface.CameraX; }
            set { Surface.CameraX = value; }
        }
        public float CameraY {
            get { return Surface.CameraY; }
            set { Surface.CameraY = value; }
        }

        public Vector2 Camera {
            get { return new Vector2(CameraX, CameraY); }
            set { CameraX = value.X; CameraY = value.Y; }
        }

        public float CameraZoom {
            get { return Surface.CameraZoom; }
            set { Surface.CameraZoom = value; }
        }

        public float CameraRotation {
            get { return Surface.CameraRotation; }
            set { Surface.CameraRotation = value; }
        }

        internal void WhenReady(Action action) {
            if (IsInitialized) action();
            else OnInit += action;
        }

        internal void Initialize() {
            Surface = new Surface(Width, Height);
            Surface.GameOverride = this;
            Surface.CenterOrigin();
            
            Draw.DefaultTargetSurface = Surface;
            Draw.ResetTarget();
            UpdateWindow();

            OnInit();

            IsInitialized = true;
        }

        internal void Run() {
            FrameTime = 1f / TargetFramesPerSecond;
            var skipTime = FrameTime * 2; // Run until we hit half the desired framerate.

            PerformanceTracker.Update(RealDeltaTime);

            if (IsFixedTimeStep) {
                elapsedTime += RealDeltaTime;
                if (elapsedTime > skipTime) elapsedTime = skipTime; // Slow down if we need to.
                while (elapsedTime >= FrameTime) {
                    elapsedTime -= FrameTime;
                    Update();
                }
            }
            else {
                Update();
            }

            ElaspedFrames++;

            if (ShowPerformanceInTitle) {
                TitleExtra = string.Format(" {1:00.000}ms {0}fps ({2:00.000}rdt | {3:00.000}ut | {4:00.000}rt) ({5}/{6}MB)",
                    Util.Min(PerformanceTracker.FramesPerSecond, TargetFramesPerSecond),
                    Util.Max(RealDeltaTime, 0.0167f) * 1000,
                    RealDeltaTime * 1000,
                    UpdateTime * 1000,
                    RenderTime * 1000,
                    GC.GetTotalMemory(false) / 1024 / 1024,
                    Process.PrivateMemorySize64 / 1024 / 1024
                    );
            }
        }

        internal void PreUpdate() {
            OnPreUpdate();
        }

        internal void PostUpdate() {
            OnPostUpdate();
        }

        internal void Update() {
            PreUpdate();

            HandleSceneSwitch();
            Input.Update();
            Tweener.Update(DeltaTime);
            Coroutine.Update(DeltaTime);
            OnUpdate();
            if (Scene != null) {
                Scene.UpdateInternal();
            }
            HasUpdatedOnce = true;

            PostUpdate();
            Input.PostUpdate();
        }

        internal void Render() {
            OnRender();
            if (Scene != null) {
                Scene.RenderInternal();
            }

            PerformanceTracker.Render();
        }

        void HandleSceneSwitch() {
            if (Scene == BufferedScene) return;

            if (Scene != null) {
                Scene.End();
                Scene.UpdateLists();
                Scene.Game = null;
            }

            BufferedScene.Game = this;
            Scene = BufferedScene;
            Scene.UpdateLists();
            Scene.Begin();
        }

    }

    class PerformanceTracker {
        public float Elasped;
        public int FrameCounter;
        public int FramesPerSecond;

        public void Update(float dt) {
            Elasped += dt;
            if (Elasped >= 1) {
                Elasped -= 1;
                FramesPerSecond = FrameCounter;
                FrameCounter = 0;
            }
        }

        public void Render() {
            FrameCounter++;
        }
    }
}
