using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Scene {
        public List<Entity> Entities { get; private set; }

        SortedDictionaryList<int, Entity> layers;
        SortedDictionaryList<int, Entity> orders;

        List<Entity> entitiesToAdd = new List<Entity>();
        List<Entity> entitiesToRemove = new List<Entity>();

        SortedDictionaryList<Enum, Collider> colliders = new SortedDictionaryList<Enum, Collider>();

        public Game Game { get; internal set; }
        public Input Input { get { return Game.Input; } }
        public Draw Draw { get { return Game.Draw; } }

        public Tweener Tweener = new Tweener();
        public Tween Tween<T>(T target, object values, float duration, float delay = 0, bool overwrite = false) where T : class {
            return Tweener.Tween(target, values, duration, delay, overwrite);
        }

        public float Timer;

        public Scene() {
            Entities = new List<Entity>();
            layers = new SortedDictionaryList<int, Entity>(new DescendingComparer<int>());
            orders = new SortedDictionaryList<int, Entity>(new DescendingComparer<int>());
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

        public IEnumerable<T> AddRange<T>(IEnumerable<T> entities) where T : Entity {
            foreach (var e in entities)
                Add(e);

            return entities;
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

            foreach(var order in orders) {
                foreach(var e in order.Value) {
                    e.UpdateFirstInternal();
                }
            }
            UpdateFirst();

            Tweener.Update(Game.DeltaTime);
            foreach (var order in orders) {
                foreach (var e in order.Value) {
                    e.UpdateInternal();
                }
            }
            Update();

            foreach (var order in orders) {
                foreach (var e in order.Value) {
                    e.UpdateLastInternal();

                    if (!layers.HasItem(e.Layer, e)) {
                        layers.RemoveItem(layers.FindKey(e), e);
                        layers.AddItem(e.Layer, e);
                    }
                    if (!orders.HasItem(e.Order, e)) {
                        orders.RemoveItem(orders.FindKey(e), e);
                        orders.AddItem(e.Order, e);
                    }
                }
            }
            UpdateLast();

            Timer += Game.DeltaTime;
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
            var found = new List<T>(Entities.Where(e => e is T).Cast<T>());
            found.AddRange(entitiesToAdd.Where(e => e is T).Cast<T>());
            return found;
        }

        public IEnumerable<Collider> GetCollidersTagged(Enum tag) {
            if (colliders.ContainsKey(tag))
                return colliders[tag];
            return new List<Collider>();
        }

        internal bool IsTagRegistered(Enum tag, Collider collider) {
            //if (!colliders.ContainsKey(tag)) return false;
            //return colliders[tag].Contains(collider);
            return colliders.HasItem(tag, collider);
        }

        internal void AddTag(Enum tag, Collider collider) {
            //if (!colliders.ContainsKey(tag))
            //    colliders.Add(tag, new List<Collider>());
            //colliders[tag].Add(collider);
            colliders.AddItem(tag, collider);
        }

        internal void RemoveTag(Enum tag, Collider collider) {
            colliders.RemoveItem(tag, collider);
        }

        internal void UpdateLists() {
            while (entitiesToRemove.Count > 0) {
                var e = entitiesToRemove[0];
                entitiesToRemove.RemoveAt(0);

                Entities.Remove(e);
                layers.RemoveItem(e.Layer, e);
                orders.RemoveItem(e.Order, e);

                e.Removed();
                e.OnRemoved();
                e.Scene = null;
            }

            while (entitiesToAdd.Count > 0) {
                var e = entitiesToAdd[0];
                entitiesToAdd.RemoveAt(0);

                Entities.Add(e);
                layers.AddItem(e.Layer, e);
                orders.AddItem(e.Order, e);

                e.Scene = this;

                var colliders = e.GetComponents<Collider>();
                foreach(var c in colliders) {
                    foreach(var t in c.Tags) {
                        AddTag(t, c);
                    }
                }

                e.Added();
                e.OnAdded();
            }
        }

        internal void RenderInternal() {
            foreach(var layer in layers) {
                foreach(var e in layer.Value) {
                    if (e.AutoRender)
                        e.RenderInternal();
                }
            }
            
            Render();
        }

        public void Render() {

        }

        internal void BringToFront(Entity e) {
            if (!layers.ContainsKey(e.Layer)) return;
            if (!layers[e.Layer].Contains(e)) return;

            layers[e.Layer].Remove(e);
            layers[e.Layer].Add(e);
        }

        internal void SendToBack(Entity e) {
            if (!layers.ContainsKey(e.Layer)) return;
            if (!layers[e.Layer].Contains(e)) return;

            layers[e.Layer].Remove(e);
            layers[e.Layer].Insert(0, e);
        }

        internal void BringForward(Entity e) {
            if (!layers.ContainsKey(e.Layer)) return;
            if (!layers[e.Layer].Contains(e)) return;

            var oldIndex = layers[e.Layer].IndexOf(e);
            var newIndex = oldIndex++;
            if (newIndex >= layers[e.Layer].Count) return;

            layers[e.Layer].Remove(e);
            layers[e.Layer].InsertOrAdd(newIndex, e);
        }

        internal void SendBackward(Entity e) {
            if (!layers.ContainsKey(e.Layer)) return;
            if (!layers[e.Layer].Contains(e)) return;

            var oldIndex = layers[e.Layer].IndexOf(e);
            var newIndex = oldIndex--;
            if (newIndex < 0) return;

            layers[e.Layer].Remove(e);
            layers[e.Layer].Insert(newIndex, e);
        }
    }

    class DescendingComparer<T> : IComparer<T> where T : IComparable<T> {
        public int Compare(T x, T y) {
            return y.CompareTo(x);
        }
    }

    class SortedDictionaryList<TKey, TValue> : SortedDictionary<TKey, List<TValue>> {
        public SortedDictionaryList(IComparer<TKey> comparer) : base(comparer) { }
        public SortedDictionaryList() { }
        public bool HasItem(TKey key, TValue item) {
            if (!ContainsKey(key))
                return false;
            else
                return this[key].Contains(item);
        }
        public TKey FindKey(TValue item) {
            foreach(var key in Keys)
                if (this[key].Contains(item)) return key;
            return default(TKey);
        }
        public void AddItem(TKey key, TValue item) {
            if (!ContainsKey(key))
                Add(key, new List<TValue>());
            this[key].Add(item);
        }
        public void RemoveItem(TKey key, TValue item) {
            this[key].Remove(item);
        }
    }
}
