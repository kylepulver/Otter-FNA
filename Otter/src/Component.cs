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

        public bool IsInEntity { get { return Entity != null; } }
        public Scene Scene { get { return Entity.Scene; } }
        public Game Game { get { return Scene.Game; } }
        public Input Input { get { return Game.Input; } }
        public Draw Draw { get { return Game.Draw; } }

        public Action OnAdded = delegate { };
        public Action OnUpdate = delegate { };
        public Action OnRender = delegate { };
        public Action OnRemoved = delegate { };

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
