using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Surface : Graphic {
        internal RenderTarget2D Target;
        public Color Color = Color.White;

        public Texture Texture;

        public float CameraX;
        public float CameraY;

        public float CameraRotation;
        public float CameraZoom = 1;

        public float MouseX {
            get { return MousePosition.X; }
        }

        public float MouseY {
            get { return MousePosition.Y; }
        }

        public Vector2 MousePosition {
            get { return GetPositionOnSurface(Input.MousePosition); }
        }

        public Vector2 Camera {
            get { return new Vector2(CameraX, CameraY); }
            set { CameraX = value.X; CameraY = value.Y; }
        }

        public Surface(int width, int height) {
            Resources.OnGraphicsReady((gd) => {
                Target = new RenderTarget2D(gd, width, height);
                Texture = new Texture(Target);
                Width = width;
                Height = height;
            });
        }

        public override void Render() {
            Draw.Texture(Texture, Texture.Bounds, RenderPosition, Scale, Rotation, Origin, Color, Shader);
        }

        public Vector2 GetPositionOnSurface(Vector2 p) {
            var invert = Matrix.Invert(TransformMatrix) * Matrix.Invert(GetCameraTransform());
            return Vector2.Transform(p + Center, invert);
        }

        public Vector2 GetPositionOnSurface(float x, float y) {
            return GetPositionOnSurface(new Vector2(x, y));
        }

        internal Matrix GetCameraTransform() {
            Console.WriteLine("{0} {1}", CameraX, Width);
            return
                Matrix.CreateTranslation(-CameraX, -CameraY, 0) *
                Matrix.CreateRotationZ(CameraRotation * Util.DEG_TO_RAD) *
                Matrix.CreateScale(CameraZoom, CameraZoom, 1);
                //Matrix.CreateTranslation(Width, HalfHeight, 0);
        }
    }
}
