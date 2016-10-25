using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Graphic : Component {

        public bool NeedsUpdate = true;
        public bool IsRelative = true;

        protected virtual void UpdateData() {

        }

        void UpdateDataIfNeeded() {
            if (NeedsUpdate) NeedsUpdate = false;
            UpdateData();
        }

        public float X;
        public float Y;

        public Vector2 Position {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public float ScaleX;
        public float ScaleY;

        public float ScaleXY {
            set { ScaleX = value; ScaleY = value; }
        }

        public Vector2 Scale {
            get { return new Vector2(ScaleX, ScaleY); }
            set { ScaleX = value.X; ScaleY = value.Y; }
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
                    return Position - Origin;
                if (!IsInEntity)
                    return Position - Origin;
                return Entity.Position + Position - Origin;
            }
        }

        public override void Render() {
            UpdateDataIfNeeded();
            base.Render();
        }
    }
}
