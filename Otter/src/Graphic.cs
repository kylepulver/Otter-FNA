using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Graphic : Component {

        public bool NeedsUpdate = true;
        public bool IsRelative = true;
        public bool IsSmoothed = true;

        protected virtual void UpdateData() {

        }

        void UpdateDataIfNeeded() {
            if (NeedsUpdate) NeedsUpdate = false;
            UpdateData();
        }

        public Shader Shader;

        public float X;
        public float Y;

        public Vector2 Position {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public float ScaleX = 1;
        public float ScaleY = 1;

        public float ScaleXY {
            set { ScaleX = value; ScaleY = value; }
        }

        public Vector2 Scale {
            get { return new Vector2(ScaleX, ScaleY); }
            set { ScaleX = value.X; ScaleY = value.Y; }
        }

        public float ScaledWidth {
            get { return Width * ScaleX; }
            set { ScaleX = value / Width; }
        }
        
        public float ScaledHeight {
            get { return Height * ScaleY; }
            set { ScaleY = value / Height; }
        }

        public Vector2 ScaledBounds {
            get { return new Vector2(ScaledWidth, ScaledHeight); }
            set { ScaledWidth = value.X; ScaledHeight = value.Y; }
        }

        public float Rotation;

        public float OriginX;
        public float OriginY;

        public float OriginXY {
            set { OriginX = value; OriginY = value; }
        }

        public Vector2 Origin {
            get { return new Vector2(OriginX, OriginY); }
            set { OriginX = value.X; OriginY = value.Y; }
        }

        public void CenterOrigin() {
            OriginX = HalfWidth;
            OriginY = HalfHeight;
        }

        public Vector2 Center {
            get { return new Vector2(HalfWidth, HalfHeight); }
        }

        public float Width;
        public float Height;

        public Vector2 Bounds {
            get { return new Vector2(Width, Height); }
            set { Width = value.X; Height = value.Y; }
        }

        public float HalfWidth { get { return Width / 2; } }
        public float HalfHeight { get { return Height / 2; } }

        public float ScrollX = 1;
        public float ScrollY = 1;

        public float ScrollXY { 
            set { ScrollX = value; ScrollY = value; }
        }

        public Vector2 Scroll {
            get { return new Vector2(ScrollX, ScrollY); }
            set { ScrollX = value.X; ScrollY = value.Y; }
        }

        public Vector2 RenderPosition {
            get {
                if (!IsRelative)
                    return Position;
                if (!IsInEntity)
                    return Position;
                return Entity.Position + Position;
            }
        }

        public Matrix TransformMatrix {
            get {
                return
                    Matrix.CreateTranslation(-Origin.X, -Origin.Y, 0) *
                    Matrix.CreateRotationZ(-Rotation * Util.DEG_TO_RAD) *
                    Matrix.CreateScale(Scale.X, Scale.Y, 1) *
                    Matrix.CreateTranslation(X + Origin.X, Y + Origin.Y, 0);
            }
        }

        public override void Render() {
            UpdateDataIfNeeded();
            base.Render();
        }
    }
}
