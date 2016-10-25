using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Transform {

        public Transform Parent { get; private set; }

        public List<Transform> Children = new List<Transform>();

        public Matrix Matrix;

        public Vector2 Position {
            get {
                Update();
                return Vector2.Transform(Vector2.Zero, Matrix);
            }
        }

        public Vector2[] Positions(params Vector2[] positions) {
            Update();
            var v = new Vector2[positions.Length];
            for (int i = 0; i < positions.Length; i++) {
                v[i] = Vector2.Transform(positions[i], Matrix);
            }
            return v;
        }

        public Transform(float x = 0, float y = 0, float rotation = 0, float scaleX = 1, float scaleY = 1, float originX = 0, float originY = 0) {
            _x = x;
            _y = y;
            _rotation = rotation;
            _scaleX = scaleX;
            _scaleY = scaleY;
            _originX = originX;
            _originY = originY;
        }

        public void Add(Transform t) {
            Children.Add(t);
            t.NeedsUpdate = true;
            t.Parent = this;
            t.Update();
        }

        public void Remove(Transform t) {
            Children.Remove(t);
            t.NeedsUpdate = true;
            t.Parent = null;
            t.Update();
        }

        public bool NeedsUpdate = true;

        float _rotation;
        public float Rotation {
            set {
                _rotation = value;
                NeedsUpdate = true;
            }
            get { return _rotation; }
        }
        float _x;
        public float X {
            set {
                _x = value;
                NeedsUpdate = true;
            }
            get { return _x; }
        }
        float _y;
        public float Y {
            set {
                _y = value;
                NeedsUpdate = true;
            }
            get { return _y; }
        }
        float _originX;
        public float OriginX {
            set {
                _originX = value;
                NeedsUpdate = true;
            }
            get { return _originX; }
        }
        float _originY;
        public float OriginY {
            set {
                _originY = value;
                NeedsUpdate = true;
            }
            get { return _originY; }
        }
        float _scaleX = 1;
        public float ScaleX {
            set {
                _scaleX = value;
                NeedsUpdate = true;
            }
            get { return _scaleX; }
        }
        float _scaleY = 1;
        public float ScaleY {
            set {
                _scaleY = value;
                NeedsUpdate = true;
            }
            get { return _scaleY; }
        }

        void UpdateMatrix(Matrix parent) {
            Matrix = Matrix.CreateTranslation(OriginX, OriginY, 0) *
                Matrix.CreateRotationZ(-Rotation * Util.DEG_TO_RAD) *
                Matrix.CreateScale(ScaleX, ScaleY, 1) *
                Matrix.CreateTranslation(X - OriginX, Y - OriginY, 0) *
                parent;

            UpdateChildren();
        }

        void UpdateChildren() {
            foreach (var c in Children) {
                c.UpdateMatrix(Matrix);
            }
        }

        public void Update() {
            if (NeedsUpdate) {
                NeedsUpdate = false;
                UpdateMatrix(Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(1, 1, 1));
            }
        }
    }
}
