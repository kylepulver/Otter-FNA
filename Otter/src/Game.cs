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

        public Action OnUpdate = delegate { };
        public Action OnRender = delegate { };
        public Action OnInit = delegate { };

        public bool IsFixedTimeStep;

        public Color Color;

        public Surface Surface;

        public List<Scene> Scenes = new List<Scene>();

        internal Scene BufferedScene;

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

        public void SwitchScene(Scene scene) {
            BufferedScene = scene;
        }

        public void Start(Scene scene = null) {
            if (scene != null)
                SwitchScene(scene);

            Core.Run();
        }

        public void Initialize() {
            Surface = new Surface(Width, Height);

            OnInit();
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

            if (ShowPerformanceInTitle) {
                TitleExtra = string.Format(" {1:00.000}ms {0}fps ({2:00.000}rdt | {3:00.000}ut | {4:00.000}rt) ({5}/{6}MB)",
                    Util.Min(FramesPerSecond, TargetFramesPerSecond),
                    Util.Max(RealDeltaTime, 0.0167f) * 1000,
                    RealDeltaTime * 1000,
                    UpdateTime * 1000,
                    RenderTime * 1000,
                    GC.GetTotalMemory(false) / 1024 / 1024,
                    Process.PrivateMemorySize64 / 1024 / 1024
                    );
            }
        }

        public void Update() {
            HandleSceneSwitch();
            Input.Update();
            Tweener.Update(DeltaTime);
            Coroutine.Update(DeltaTime);
            OnUpdate();
            if (Scene != null) {
                Scene.UpdateInternal();
            }
            HasUpdatedOnce = true;
        }

        public void Render() {
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
