using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Surface : Graphic {
        internal RenderTarget2D Target;

        public float CameraX;
        public float CameraY;

        public float CameraRotation;
        public float CameraZoom = 1;

        public Vector2 Camera {
            get { return new Vector2(CameraX, CameraY); }
            set { CameraX = value.X; CameraY = value.Y; }
        }

        public Surface(int width, int height) {
            Resources.OnGraphicsReady((gd) => {
                Target = new RenderTarget2D(gd, width, height);
            });
        }

        internal Matrix GetCameraTransform() {
            return
                Matrix.CreateTranslation(-CameraX - HalfWidth, -CameraY - HalfHeight, 0) *
                Matrix.CreateRotationZ(-CameraRotation * Util.DEG_TO_RAD) *
                Matrix.CreateScale(CameraZoom, CameraZoom, 1) *
                Matrix.CreateTranslation(Game.Instance.HalfWidth, Game.Instance.HalfHeight, 0);
        }

    }
}
