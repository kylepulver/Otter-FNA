using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Scene {
        public List<Entity> Entities { get; private set; }

        SortedDictionary<int, List<Entity>> layers = new SortedDictionary<int, List<Entity>>();
        SortedDictionary<int, List<Entity>> orders = new SortedDictionary<int, List<Entity>>();

        List<Entity> entitiesToAdd = new List<Entity>();
        List<Entity> entitiesToRemove = new List<Entity>();

        public Game Game { get; internal set; }
        public Input Input { get { return Game.Input; } }
        public Draw Draw { get { return Game.Draw; } }

        

        public Tweener Tweener = new Tweener();
        public Tween Tween<T>(T target, object values, float duration, float delay = 0, bool overwrite = false) where T : class {
            return Tweener.Tween(target, values, duration, delay, overwrite);
        }

        public Scene() {
            Entities = new List<Entity>();
        }

        void ChangeLayer(Entity e) {

        }

        void ChangeOrder(Entity e) {

        }

        public T Add<T>(T e) where T : Entity {
            if (e.IsInScene)
                throw new ArgumentException("Entity already belongs to a Scene.");

            if (entitiesToRemove.Contains(e))
                entitiesToRemove.Remove(e);
            else 
                entitiesToAdd.Add(e);

            return e;
        }

        public T Remove<T>(T e) where T : Entity {
            if (!Entities.Contains(e))
                throw new ArgumentException("Entity isn't in Scene.");

            if (entitiesToAdd.Contains(e))
                entitiesToAdd.Remove(e);
            else
                entitiesToRemove.Add(e);

            return e;
        }

        internal void UpdateInternal() {
            UpdateLists();

            foreach (var e in Entities) {
                e.UpdateFirstInternal();
            }
            UpdateFirst();

            Tweener.Update(Game.DeltaTime);
            foreach (var e in Entities) {
                e.UpdateInternal();
            }
            Update();

            foreach (var e in Entities) {
                e.UpdateLastInternal();
            }
            UpdateLast();
        }

        public virtual void UpdateFirst() {

        }

        public virtual void UpdateLast() {

        }

        public virtual void Update() {

        }

        public virtual void Begin() {

        }

        public virtual void End() {

        }

        public T GetEntity<T>() where T : Entity {
            foreach (var e in Entities) {
                if (e is T) return (T)e;
            }
            foreach(var e in entitiesToAdd) {
                if (e is T) return (T)e; // Returns entity not actually in scene yet. EXPERIMENTAL!!111
            }
            return null;
        }

        public IEnumerable<T> GetEntities<T>() where T : Entity {
            return Entities.Where(e => e.GetType() == typeof(T)).Cast<T>();
        }

        public void UpdateLists() {
            while (entitiesToRemove.Count > 0) {
                var e = entitiesToRemove[0];
                entitiesToRemove.RemoveAt(0);

                Entities.Remove(e);
                e.Removed();
                e.OnRemoved();
                e.Scene = null;
            }

            while (entitiesToAdd.Count > 0) {
                var e = entitiesToAdd[0];
                entitiesToAdd.RemoveAt(0);

                Entities.Add(e);
                e.Scene = this;
                e.OnAdded();
                e.Added();
            }
        }

        internal void RenderInternal() {
            foreach (var e in Entities) {
                e.RenderInternal();
            }
            Render();
        }

        public void Render() {

        }
    }

    
}
