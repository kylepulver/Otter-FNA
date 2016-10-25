using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Otter {
    public class Shader {

        internal Effect XnaEffect;

        public string Path { get; private set; }

        public Shader(string path) {
            Path = path;

            Resources.OnGraphicsReady((gd) => {
                XnaEffect = new Effect(gd, File.ReadAllBytes(Path));
            });
        }
    }
}
