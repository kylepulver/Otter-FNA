using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Surface : Component {
        internal RenderTarget2D Target;

        public float CameraX;
        public float CameraY;

        public Vector2 Camera {
            get { return new Vector2(CameraX, CameraY); }
            set { CameraX = value.X; CameraY = value.Y; }
        }

        public Surface(int width, int height) {
            Target = new RenderTarget2D(Core.Instance.GraphicsDevice, width, height);
        }

    }
}
