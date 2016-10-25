using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Sprite<TAnimType> : Image {

        public int CellWidth;
        public int CellHeight;
        public int FrameIndex;

        void Initialize(int cellWidth, int cellHeight) {
            CellWidth = cellWidth;
            CellHeight = cellHeight;
        }

        public Sprite(Texture texture, int cellWidth, int cellHeight)
            : base(texture) {
                Initialize(cellWidth, cellHeight);
        }

        public Sprite(string path, int cellWidth, int cellHeight)
            : base(path) {
                Initialize(cellWidth, cellHeight);
        }

        public override void Render() {
            var x = Util.TwoDeeX(FrameIndex, Texture.GetColumns(CellWidth)) * CellWidth;
            var y = Util.TwoDeeY(FrameIndex, Texture.GetColumns(CellWidth)) * CellHeight;    

            Draw.Texture(Texture, x, y, CellWidth, CellHeight, RenderPosition, Color);
        }
    }

    public class Sprite : Sprite<int> {
        public Sprite(Texture texture, int cellWidth, int cellHeight)
            : base(texture, cellWidth, cellHeight) {

        }

        public Sprite(string path, int cellWidth, int cellHeight)
            : base(path, cellWidth, cellHeight) {

        }
    }
}
