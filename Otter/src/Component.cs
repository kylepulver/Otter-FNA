using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Component {

        public Entity Entity { get; internal set; }

        public bool IsVisible = true;
        public bool IsEnabled = true;
        public bool IsInitialized;

        public float Timer;

        public Scene Scene { get { return Entity.Scene; } }
        public Game Game {
            set {
                GameOverride = value;
            }
            get {
                if (GameOverride != null) return GameOverride;
                if (Entity == null)
                    return Game.Instance; // Might be a weird assumption but whatever.
                if (Scene == null)
                    return Game.Instance; // Might be a weird assumption but whatever.
                return Scene.Game;
            }
        }
        public Input Input { get { return Game.Input; } }
        public Draw Draw { get { return Game.Draw; } }

        // Special case for graphics that aren't attached to entities.
        internal Game GameOverride;

        public bool IsInEntity { get { return Entity != null; } }
        public bool IsInScene {
            get {
                if (!IsInEntity) return false;
                return (Entity.IsInScene);
            }
        }

        public Action OnAdded = delegate { };
        public Action OnUpdate = delegate { };
        public Action OnRender = delegate { };
        public Action OnRemoved = delegate { };

        internal int UpdateFramestamp = 0;

        public T GetEntity<T>() where T : Entity {
            return (T)Entity;
        }

        /// <summary>
        /// Call after constructing to take care of any general initialization stuff.
        /// Will set IsInitialized to true.
        /// </summary>
        public virtual void Initialize() {
            IsInitialized = true;
        }

        internal void UpdateInternal() {
            if (!IsEnabled) return;
            Update();
            OnUpdate();
            Timer += Game.DeltaTime;
            UpdateFramestamp = Game.ElaspedFrames;
        }

        internal void RenderInternal() {
            if (!IsVisible) return;
            Render();
            OnRender();
        }

        public virtual void Added() {

        }

        public virtual void Removed() {

        }

        public virtual void Update() {

        }

        public virtual void Render() {

        }
    }
}
