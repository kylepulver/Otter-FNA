using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class PixelCollider : Collider {

        public Color[] Pixels;

        public float Threshold = 0.5f;

        public PixelCollider(Texture texture) {
            Width = texture.Bounds.Width;
            Height = texture.Bounds.Height;

            Pixels = new Color[(int)Width * (int)Height];

            Microsoft.Xna.Framework.Color[] xnaPixels = new Microsoft.Xna.Framework.Color[(int)Width * (int)Height];
            texture.XnaTexture.GetData(xnaPixels);
            for(var i = 0; i < xnaPixels.Length; i++) {
                Pixels[i] = xnaPixels[i];
            }
        }

        public PixelCollider(string path) : this(new Texture(path)) { }

        public bool GetPixel(int x, int y) {
            if (x < 0 || y < 0 || x >= Width || y >= Height)
                return false;

            var index = Util.OneDee((int)Width, x, y);
            var pixel = Pixels[index];
            return pixel.A >= Threshold;
        }

        public bool GetRect(float x, float y, float x2, float y2) {
            for (int i = (int)x; i <= x2; i++) {
                for (int j = (int)y; j <= y2; j++) {
                    if (GetPixel(i, j))
                        return true;
                }
            }
            return false;
        }
    }
}
