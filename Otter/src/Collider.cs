using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Collider : Component {

        public Vector2 Position {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public float X {
            get {
                if (IsRelative) return LocalX + Entity.X - OriginX;
                return LocalX;
            }
            set {
                LocalX = value;
            }
        }
        public float Y {
            get {
                if (IsRelative) return LocalY + Entity.Y - OriginY;
                return LocalY;
            }
            set {
                LocalY = value;
            }
        }

        public float OriginX;
        public float OriginY;

        public void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = HalfHeight;
        }

        public float HalfWidth { get { return Width / 2; } }
        public float HalfHeight { get { return Height / 2; } }

        public float LocalX;
        public float LocalY;

        public bool IsRelative = true;

        public float Width;
        public float Height;

        public float Left {
            get { return X; }
        }
        public float Right {
            get { return X + Width - 1; } // Adding -1 might be a mistake :I
        }
        public float Top {
            get { return Y; }
        }
        public float Bottom {
            get { return Y + Height - 1; } // Dunno about this :I
        }
        
        public float CenterX {
            get { return X + HalfWidth; }
        }
        public float CenterY {
            get { return Y + HalfHeight; }
        }

        public Bounds Bounds {
            get { return new Bounds(Left, Top, Width, Height); }
        }

        public List<Enum> Tags = new List<Enum>();

        delegate bool CheckAgainstMethod(Collider c1, Collider c2);

        static Dictionary<Type, Dictionary<Type, CheckAgainstMethod>> collisionMethods = new Dictionary<Type, Dictionary<Type, CheckAgainstMethod>>();

        static CheckAgainstMethod CreateCollisionMethod(MethodInfo mi) {
            return (CheckAgainstMethod)Delegate.CreateDelegate(typeof(CheckAgainstMethod), null, mi);
        }

        static Collider() {
            // Grab methods with the ColliderMethod attribute
            AppDomain.CurrentDomain.GetAssemblies()
                .Each(a => a.GetTypes()
                    .Each(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .Each(m => {
                            if (m.IsDefined(typeof(CollisionMethodAttribute), false)) {
                                var methodParams = m.GetParameters();
                                var attr = m.GetCustomAttribute<CollisionMethodAttribute>();
                                var t1 = attr.T1;
                                var t2 = attr.T2;
                                var collisionMethod = CreateCollisionMethod(m);

                                if (!collisionMethods.ContainsKey(t1)) {
                                    collisionMethods.Add(t1, new Dictionary<Type, CheckAgainstMethod>());
                                }
                                if (!collisionMethods[t1].ContainsKey(t2)) {
                                    collisionMethods[t1].Add(t2, collisionMethod);
                                }
                            }
                        })));
        }

        public void AddTag(Enum tag) {
            Tags.Add(tag);
            if (IsInScene) {
                Scene.RemoveTag(tag, this);
            }
        }

        public void RemoveTag(Enum tag) {
            Tags.Remove(tag);
            if (IsInScene) {
                Scene.AddTag(tag, this);
            }
        }

        public IEnumerable<Collider> CollideAll(float atX, float atY, Enum tag) {
            return CollideAll(atX, atY, Scene.GetCollidersTagged(tag));
        }

        public IEnumerable<Collider> CollideAll(float atX, float atY, Entity e) {
            return CollideAll(atX, atY, e.GetComponents<Collider>());
        }

        public IEnumerable<Collider> CollideAll(float atX, float atY, IEnumerable<Entity> testAgainst) {
            var found = new List<Collider>();
            foreach(var e in testAgainst) {
                found.AddRange(CollideAll(atX, atY, e));
            }
            return found;
        }

        public IEnumerable<Collider> CollideAll(float atX, float atY, IEnumerable<Collider> testAgainst) {
            var found = new List<Collider>();
            foreach (var c in testAgainst) {
                if (Collide(atX, atY, c) != null)
                    found.Add(c);
            }
            return found;
        }

        public Collider Collide(float atX, float atY, Enum tag) {
            return Collide(atX, atY, Scene.GetCollidersTagged(tag));
        }

        public Collider Collide(float atX, float atY, Entity e) {
            return Collide(atX, atY, e.GetComponents<Collider>());
        }

        public bool Overlap(float atX, float atY, Entity e) {
            return Collide(atX, atY, e) != null;
        }

        public Collider Collide(float atX, float atY, IEnumerable<Entity> testAgainst) {
            foreach(var e in testAgainst) {
                var c = Collide(atX, atY, e);
                if (c != null)
                    return c;
            }
            return null;
        }

        public Collider Collide(float atX, float atY, IEnumerable<Collider> testAgainst) {
            foreach (var c in testAgainst) {
                if (Collide(atX, atY, c) != null)
                    return c;
            }
            return null;
        }

        public bool Overlap(float atX, float atY, Enum tag) {
            return Collide(atX, atY, tag) != null;
        }

        public Collider Collide(float atX, float atY, Collider other) {
            if (Entity == null) throw new NullReferenceException("Must be added to an Entity before collision checks.");
            if (this == other) return null; // Dont collide with yourself, dummy.
            Collider result = null;

            var startX = Entity.X;
            var startY = Entity.Y;

            Entity.X = atX;
            Entity.Y = atY;

            if (CheckAgainst(other)) result = other;

            Entity.X = startX;
            Entity.Y = startY;

            return result;
        }

        public bool Overlap(float atX, float atY, Collider other) {
            return Collide(atX, atY, other) != null;
        }

        public bool CheckAgainst<T>(T other) where T : Collider {
            var t1 = GetType();
            var t2 = other.GetType();

            if (!collisionMethods.ContainsKey(t1)) {
                if (!collisionMethods.ContainsKey(t2)) {
                    return false;
                }
            }
            else {
                if (!collisionMethods[t1].ContainsKey(t2)) {
                    if (collisionMethods[t2].ContainsKey(t1)) {
                        return collisionMethods[t2][t1](other, this);
                    }
                }
            }

            if (!collisionMethods[t1].ContainsKey(t2)) return false;
            return collisionMethods[t1][t2](this, other);
        }

        class CollisionMethodAttribute : Attribute {
            public Type T1;
            public Type T2;
        }

        static bool Overlap(RectCollider A, RectCollider B) {
            return Overlap(A.Bounds, B.Bounds);
        }

        static bool Overlap(Bounds A, Bounds B) {
            if (A.Left > B.Right) return false;
            if (A.Top > B.Bottom) return false;
            if (B.Left > A.Right) return false;
            if (B.Top > B.Bottom) return false;

            return true;
        }

        static bool Overlap(CircleCollider A, CircleCollider B) {
            var radii = A.Radius + B.Radius;
            var dist = Util.Distance(A.X, A.Y, B.X, B.Y);

            return dist <= radii;
        }

        [CollisionMethod(T1 = typeof(RectCollider), T2 = typeof(RectCollider))]
        static bool RectRect(Collider A, Collider B) {
            var rectA = (RectCollider)A;
            var rectB = (RectCollider)B;

            return Overlap(rectA, rectB);
        }

        [CollisionMethod(T1 = typeof(RectCollider), T2 = typeof(CircleCollider))]
        static bool RectCircle(Collider A, Collider B) {
            var rect = (RectCollider)A;
            var circ = (CircleCollider)B;

            if (Util.PointInRect(circ.CenterX, circ.CenterY, rect.Left, rect.Top, rect.Width, rect.Height))
                return true;

            if (Util.DistancePointRect(circ.CenterX, circ.CenterY, rect.Left, rect.Top, rect.Width, rect.Height) < circ.Radius)
                return true;

            Line2 boxLine;

            boxLine = new Line2(rect.Left, rect.Top, rect.Right, rect.Top);
            if (boxLine.IntersectCircle(new Vector2(circ.CenterX, circ.CenterY), circ.Radius)) return true;

            boxLine = new Line2(rect.Right, rect.Top, rect.Right, rect.Bottom);
            if (boxLine.IntersectCircle(new Vector2(circ.CenterX, circ.CenterY), circ.Radius)) return true;

            boxLine = new Line2(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            if (boxLine.IntersectCircle(new Vector2(circ.CenterX, circ.CenterY), circ.Radius)) return true;

            boxLine = new Line2(rect.Left, rect.Bottom, rect.Left, rect.Top);
            return boxLine.IntersectCircle(new Vector2(circ.CenterX, circ.CenterY), circ.Radius);
        }

        [CollisionMethod(T1 = typeof(CircleCollider), T2 = typeof(CircleCollider))]
        static bool CircleCircle(Collider A, Collider B) {
            var circA = (CircleCollider)A;
            var circB = (CircleCollider)B;

            var radii = circA.Radius + circB.Radius;
            var dist = Util.Distance(circA.X, circA.Y, circB.X, circB.Y);

            return dist <= radii;
        }

        [CollisionMethod(T1 = typeof(RectCollider), T2 = typeof(GridCollider))]
        static bool RectGrid(Collider A, Collider B) {
            var rect = (RectCollider)A;
            var grid = (GridCollider)B;

            if (!Overlap(rect.Bounds, grid.Bounds))
                return false;

            return grid.GetRect(
                rect.Left - grid.X,
                rect.Top - grid.Y,
                rect.Right - grid.X,
                rect.Bottom - grid.Y
                );
        }

        [CollisionMethod(T1 = typeof(CircleCollider), T2 = typeof(GridCollider))]
        static bool CircleGrid(Collider A, Collider B) {
            var circ = (CircleCollider)A;
            var grid = (GridCollider)B;

            if (!Overlap(circ.Bounds, grid.Bounds))
                return false;

            // for each tile in B in circ bounds, check circ vs rect on tile

            return false;
        }

        [CollisionMethod(T1 = typeof(GridCollider), T2 = typeof(GridCollider))]
        static bool GridGrid(Collider A, Collider B) {
            var gridA = (GridCollider)A;
            var gridB = (GridCollider)B;

            if (!Overlap(gridA.Bounds, gridB.Bounds))
                return false;

            // for every tile in A, make a rect, check against grid B, return true on first touch

            return false;
        }
    }

}
