using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Image : Graphic {
        public Color Color = Otter.Color.White;
        public Shader Shader;

        public Texture Texture;

        public Image(string path) {
            Texture = new Texture(path);
        }

        public Image(Texture texture) {
            Texture = texture;
        }

        public override void Render() {
            //Draw.Texture(Texture, Texture.Bounds, RenderPosition, Color, Shader);
            Draw.Texture(Texture, Texture.Bounds, RenderPosition, Color);
        }
    }
}
