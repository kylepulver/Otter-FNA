using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Collider : Component {

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
            get { return X + OriginX; }
        }
        public float Right {
            get { return X + OriginX + Width; }
        }
        public float Top {
            get { return Y + OriginY; }
        }
        public float Bottom {
            get { return Y + OriginY + Height; }
        }

        delegate bool CheckAgainstMethod(Collider c1, Collider c2);

        static bool isInit;

        static Dictionary<Type, Dictionary<Type, CheckAgainstMethod>> collisionMethods = new Dictionary<Type, Dictionary<Type, CheckAgainstMethod>>();

        static CheckAgainstMethod CreateCollisionMethod(MethodInfo mi) {
            return (CheckAgainstMethod)Delegate.CreateDelegate(typeof(CheckAgainstMethod), null, mi);
        }

        static Collider() {
            //AppDomain.CurrentDomain.GetAssemblies()
            //    .Each(a => a.GetTypes()
            //    .Where(t => t.IsSubclassOf(typeof(Collider)))
            //        .Each(t => {
            //            collisionMethods.Add(t, new Dictionary<Type, CheckAgainstMethod>());
            //        })
            //    );

            // Grab methods with the ColliderMethod attribute
            AppDomain.CurrentDomain.GetAssemblies()
                .Each(a => a.GetTypes()
                    .Each(t => t.GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                        .Each(m => {
                            if (m.IsDefined(typeof(CollisionMethodAttribute), false)) {
                                Console.WriteLine(m.Name);
                                var methodParams = m.GetParameters();
                                var attr = m.GetCustomAttribute<CollisionMethodAttribute>();
                                var t1 = attr.T1;
                                var t2 = attr.T2;
                                if (!collisionMethods.ContainsKey(t1)) {
                                    collisionMethods.Add(t1, new Dictionary<Type, CheckAgainstMethod>());
                                }
                                if (!collisionMethods.ContainsKey(t2)) {
                                    collisionMethods.Add(t2, new Dictionary<Type, CheckAgainstMethod>());
                                }
                                var collisionMethod = CreateCollisionMethod(m);
                                if (!collisionMethods[t1].ContainsKey(t2)) {
                                    collisionMethods[t1].Add(t2, collisionMethod);
                                }
                                if (!collisionMethods[t2].ContainsKey(t1)) {
                                    collisionMethods[t2].Add(t1, collisionMethod);
                                }
                            }
                        })));
        }



        public Collider Collide(float atX, float atY, Collider other) {
            if (Entity == null) throw new NullReferenceException("Must be added to an Entity before collision checks.");
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

        public bool CheckAgainst<T>(T other) where T : Collider {
            var t1 = GetType();
            var t2 = other.GetType();

            if (!collisionMethods.ContainsKey(t1)) {
                if (!collisionMethods.ContainsKey(t2)) {
                    return false;
                }
                else {
                    if (!collisionMethods[t2].ContainsKey(t1)) {
                        return false;
                    }
                    else {
                        return collisionMethods[t2][t1](this, other);
                    }
                }
            }
            else {

            }
            if (!collisionMethods[t1].ContainsKey(t2)) return false;
            return collisionMethods[t1][t2](this, other);
        }

        class CollisionMethodAttribute : Attribute {
            public Type T1;
            public Type T2;
        }

        static bool Overlap(RectCollider A, RectCollider B) {
            if (A.Left > B.Right) return false;
            if (A.Top > B.Bottom) return false;
            if (B.Left > A.Right) return false;
            if (B.Top > B.Bottom) return false;

            return true;
        }

        static bool Overlap(RectCollider A, CircleCollider B) {
            return false;
        }
        static bool Overlap(CircleCollider A, RectCollider B) {
            return Overlap(B, A);
        }

        static bool Overlap(CircleCollider A, CircleCollider B) {
            var radii = A.Radius + B.Radius;
            var dist = Util.Distance(A.X, A.Y, B.X, B.Y);

            if (dist <= radii) {
                return true;
            }

            return false;
        }

        [CollisionMethod(T1 = typeof(RectCollider), T2 = typeof(RectCollider))]
        static bool RectRect(Collider A, Collider B) {
            var rectA = (RectCollider)A;
            var rectB = (RectCollider)B;

            if (rectA.Left > rectB.Right) return false;
            if (rectA.Top > rectB.Bottom) return false;
            if (rectB.Left > rectA.Right) return false;
            if (rectB.Top > rectB.Bottom) return false;

            return true;
        }

        [CollisionMethod(T1 = typeof(RectCollider), T2 = typeof(CircleCollider))]
        static bool RectCircle(Collider A, Collider B) {
            var rect = (RectCollider)A;
            var circ = (CircleCollider)B;
            
            return false;
        }

        [CollisionMethod(T1 = typeof(CircleCollider), T2 = typeof(CircleCollider))]
        static bool CircleCircle(Collider A, Collider B) {
            var circA = (CircleCollider)A;
            var circB = (CircleCollider)B;

            var radii = circA.Radius + circB.Radius;
            var dist = Util.Distance(circA.X, circA.Y, circB.X, circB.Y);

            if (dist <= radii) {
                return true;
            }

            return false;
        }
    }
}
