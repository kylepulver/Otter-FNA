using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Otter {
    public class Texture {
        internal Texture2D XnaTexture;

        public string Path { get; private set; }

        public Rectangle Bounds;

        public int GetColumns(int cellWidth) {
            return XnaTexture.Width / cellWidth;
        }

        public int GetRows(int cellHeight) {
            return XnaTexture.Height / cellHeight;
        }

        public int GetMaxCells(int cellWidth, int cellHeight) {
            return GetColumns(cellWidth) * GetRows(cellHeight);
        }

        public Texture(string path) {
            Path = path;
            Resources.OnGraphicsReady((gd) => {
                XnaTexture = Resources.GetTexture2D(Path);
                Bounds = new Rectangle(0, 0, XnaTexture.Width, XnaTexture.Height);
            });
        }

        internal Texture(Texture2D xnaTexture) {
            XnaTexture = xnaTexture;
            Bounds = new Rectangle(0, 0, XnaTexture.Width, XnaTexture.Height);
        }
    }
}
