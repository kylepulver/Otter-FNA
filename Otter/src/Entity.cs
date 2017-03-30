using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Entity {
        public List<Component> Components = new List<Component>();

        List<Component> componentsToAdd = new List<Component>();
        List<Component> componentsToRemove = new List<Component>();

        public Scene Scene { get; internal set; }
        public Game Game { get { return Scene.Game; } }
        public Input Input { get { return Game.Input; } }
        public Draw Draw { get { return Game.Draw; } }

        public bool IsInScene { get { return Scene != null; } }
        public bool HasUpdatedOnce { get; internal set; }

        public bool IsVisible = true;
        public bool IsEnabled = true;

        public Surface Surface;

        public bool AutoRender = true;

        public float X;
        public float Y;

        public float Timer;

        public int Order;
        public int Layer;

        internal int UpdateFramestamp = 0;

        public Tweener Tweener = new Tweener();
        public Tween Tween<T>(T target, object values, float duration, float delay = 0, bool overwrite = false) where T : class {
            return Tweener.Tween(target, values, duration, delay, overwrite);
        }

        public Vector2 Position {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public IEnumerable<Component> Graphics {
            get { return Components.Where(c => c is Graphic); }
        }

        public Entity()
            : this(0, 0, null, null) {

        }

        public Entity(float x, float y, Graphic g, Collider c) {
            X = x;
            Y = y;
            if (g != null)
                Graphic = g;
            if (c != null)
                Collider = c;
        }

        public Entity(float x, float y, Graphic g)
            : this(x, y, g, null) {

        }

        public Entity(float x, float y)
            : this(x, y, null, null) {

        }

        public Entity(float x, float y, Collider c)
            : this(x, y, null, c) {

        }

        public void ClearGraphics() {
            RemoveRange<Graphic>();
        }

        public void ClearColliders() {
            RemoveRange<Collider>();
        }

        public Graphic Graphic {
            get { return GetComponent<Graphic>(); }
            set { 
                ClearGraphics();
                Add(value);
            }
        }

        public Collider Collider {
            get { return GetComponent<Collider>(); }
            set {
                ClearColliders();
                Add(value);
            }
        }

        public void BringToFront() {
            Scene.BringToFront(this);
        }

        public void SendToBack() {
            Scene.SendToBack(this);
        }

        public void BringForward() {
            Scene.BringForward(this);
        }

        public void SendBackward() {
            Scene.SendBackward(this);
        }

        internal void UpdateInternal() {
            if (!IsEnabled) return;

            Tweener.Update(Game.DeltaTime);
            UpdateLists();

            Update();
            OnUpdate();

            foreach (var c in Components) {
                c.UpdateInternal();
            }

            UpdateLists();
        }

        internal void UpdateFirstInternal() {
            if (!IsEnabled) return;

            UpdateLists();
        }

        internal void UpdateLastInternal() {
            if (!IsEnabled) return;

            UpdateLists();
            Timer += Game.DeltaTime;
            UpdateFramestamp = Game.ElaspedFrames;
        }

        internal void RenderInternal() {
            if (!IsVisible) return;

            Surface previousSurface = null;
            if (Surface != null) {
                if (!Draw.IsUsingDefaultSurface)
                    previousSurface = Draw.TargetSurface;

                Draw.SetTarget(Surface);
            }

            Prerender();
            OnPrerender();

            foreach (var c in Components) {
                c.RenderInternal();
            }

            Render();
            OnRender();

            if (Surface != null) {
                if (previousSurface == null)
                    Draw.ResetTarget();
                else
                    Draw.SetTarget(previousSurface);
            }
        }

        public void AddRange(params Component[] components) {
            components.Each(c => Add(c));
        }

        public T Add<T>(T c) where T : Component {
            if (componentsToRemove.Contains(c))
                componentsToRemove.Remove(c);
            else 
                componentsToAdd.Add(c);

            if (!HasUpdatedOnce)
                UpdateLists();

            return c;
        }

        public T Remove<T>(T c) where T : Component {
            if (componentsToAdd.Contains(c))
                componentsToAdd.Remove(c);
            else
                componentsToRemove.Add(c);

            if (!HasUpdatedOnce)
                UpdateLists();

            return c;
        }

        public void RemoveRange<T>() where T : Component {
            GetComponents<T>().Each(c => Remove(c));
        }

        public T GetComponent<T>() where T : Component {
            foreach (var c in Components) {
                if (c is T) return (T)c;
            }
            foreach (var c in componentsToAdd) {
                if (c is T) return (T)c;
            }
            return null;
        }

        public T GetEntity<T>() where T : Entity {
            return Scene.GetEntity<T>();
        }

        public IEnumerable<T> GetEntities<T>() where T : Entity {
            return Scene.GetEntities<T>();
        }

        public IEnumerable<T> GetComponents<T>() where T : Component {
            var found = new List<T>(Components.Where(c => c is T).Cast<T>());
            found.AddRange(componentsToAdd.Where(c => c is T).Cast<T>());
            return found;
        }

        public void UpdateLists() {
            while (componentsToRemove.Count > 0) {
                var c = componentsToRemove[0];
                componentsToRemove.RemoveAt(0);

                Components.Remove(c);
                c.OnRemoved();
                c.Removed();
                c.Entity = null;
            }

            while (componentsToAdd.Count > 0) {
                var c = componentsToAdd[0];
                componentsToAdd.RemoveAt(0);

                Components.Add(c);
                c.Entity = this;
                c.OnAdded();
                c.Added();
            }
        }

        public Action OnUpdate = delegate { };
        public Action OnRender = delegate { };
        public Action OnPrerender = delegate { };
        public Action OnAdded = delegate { };
        public Action OnRemoved = delegate { };

        public virtual void Update() {

        }

        public virtual void UpdateFirst() {

        }

        public virtual void UpdateLast() {

        }

        public virtual void Render() {

        }

        public virtual void Prerender() {

        }

        public virtual void Added() {

        }

        public virtual void Removed() {

        }
    }
}
