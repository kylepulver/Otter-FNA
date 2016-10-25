using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public class Tiles : Graphic {

        public int Columns;
        public int Rows;
        public int TileWidth;
        public int TileHeight;
        public Color Color = 0xffffffff;

        public Texture Texture;

        public Tile[] TileData;

        public Tiles(int columns, int rows, int tileWidth, int tileHeight, Texture texture) {
            Columns = columns;
            Rows = rows;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Texture = texture;

            TileData = new Tile[columns * rows];
        }

        public int TileCount {
            get { return Columns * Rows; }
        }

        public void SetTile(int index, int tileIndex) {
            if (TileData[index] == null) {
                CreateTile(index);
            }
            TileData[index].TileIndex = tileIndex;
        }

        public void SetTile(int x, int y, int tileIndex) {
            SetTile(Util.OneDee(Columns, x, y), tileIndex);
        }

        public void ClearTile(int index) {
            TileData[index] = null;
        }

        public void ClearTile(int x, int y) {
            ClearTile(Util.OneDee(Columns, x, y));
        }

        void CreateTile(int index) {
            var c = new Tile();

            c.Index = index;
            c.Tiles = this;

            TileData[index] = c;
        }

        public override void Render() {
            base.Render();

            for (int i = 0; i < TileData.Length; i++) {
                if (TileData[i] == null) continue;

                var c = TileData[i];
                var x = Util.TwoDeeX(i, Columns) * TileWidth + RenderPosition.X;
                var y = Util.TwoDeeY(i, Columns) * TileHeight + RenderPosition.Y;

                Draw.Texture(Texture, c.SourceX, c.SourceY, TileWidth, TileHeight, x, y, Color * c.Color);
            }
        }

        public class Tile {
            public int Index;
            public int SourceX;
            public int SourceY;
            public Color Color = 0xffffffff;

            int _tileIndex;
            public int TileIndex {
                get { return _tileIndex; }
                set {
                    _tileIndex = value;
                    Resources.OnGraphicsReady((gd) => {
                        SourceX = Util.TwoDeeX(value, Tiles.Texture.GetColumns(Tiles.TileWidth)) * Tiles.TileWidth;
                        SourceY = Util.TwoDeeY(value, Tiles.Texture.GetColumns(Tiles.TileWidth)) * Tiles.TileHeight;
                    });
                }
            }

            public Tiles Tiles;
            
        }
    }
}
