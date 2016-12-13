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

        public static Tiles CreateFromPixelSize(int pixelWidth, int pixelHeight, int tileWidth, int tileHeight, Texture texture) {
            return new Tiles(pixelWidth / tileWidth, pixelHeight / tileHeight, tileWidth, tileHeight, texture);
        }

        public static Tiles CreateFromPixelSize(int pixelWidth, int pixelHeight, int tileWidth, int tileHeight, string path) {
            return CreateFromPixelSize(pixelWidth, pixelHeight, tileWidth, tileHeight, new Texture(path));
        }

        public Tiles(int columns, int rows, int tileWidth, int tileHeight, Texture texture) {
            Columns = columns;
            Rows = rows;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Texture = texture;

            Width = columns * tileWidth;
            Height = rows * tileHeight;

            TileData = new Tile[TileCount];
        }

        public Tiles(int columns, int rows, int tileWidth, int tileHeight, string path) 
            : this(columns, rows, tileWidth, tileHeight, new Texture(path)) { }

        public Tiles(int size, int tileSize, Texture texture)
            : this(size, size, tileSize, tileSize, texture) { }

        public Tiles(int size, int tileSize, string path)
            : this(size, size, new Texture(path)) { }

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

        public Tile GetTile(int index) {
            return TileData[index];
        }

        public Tile GetTile(int x, int y) {
            return GetTile(Util.OneDee(Columns, x, y));
        }

        public Tile GetTileAtPosition(float x, float y) {
            return GetTile(GetTileIndexAtPosition(x, y));
        }

        public int GetTileIndexAtPosition(float x, float y) {
            var gridX = (int)Util.SnapToGrid(x, TileWidth) / TileWidth;
            var gridY = (int)Util.SnapToGrid(y, TileHeight) / TileHeight;
            return Util.OneDee(Columns, gridX, gridY);
        }

        void CreateTile(int index) {
            var c = new Tile();

            c.Index = index;
            c.Tiles = this;

            TileData[index] = c;
        }

        public override void Render() {
            base.Render();

            Draw.End();
            Draw.Begin(RenderPosition, Scale, Rotation, Origin);
            for (int i = 0; i < TileData.Length; i++) {
                if (TileData[i] == null) continue;

                var c = TileData[i];
                var x = Util.TwoDeeX(i, Columns) * TileWidth;
                var y = Util.TwoDeeY(i, Columns) * TileHeight;

                Draw.Texture(Texture, c.SourceX, c.SourceY, TileWidth, TileHeight, x, y, c.Color * Color);
                //Draw.Texture(Texture, c.SourceX, c.SourceY, TileWidth, TileHeight, new Vector2(x, y), Scale, Rotation, Origin, Color, Shader);
            }
            Draw.End();
            Draw.Begin();
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
