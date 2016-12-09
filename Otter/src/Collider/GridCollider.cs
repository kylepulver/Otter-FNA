using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class GridCollider : Collider {

        public int Columns;
        public int Rows;
        public int TileWidth;
        public int TileHeight;

        public bool[] TileData;

        public GridCollider(int columns, int rows, int tileWidth, int tileHeight) {
            Columns = columns;
            Rows = rows;
            TileWidth = tileWidth;
            TileHeight = tileHeight;

            Width = columns * tileWidth;
            Height = rows * tileHeight;

            TileData = new bool[TileCount];
        }

        public int TileCount {
            get { return Columns * Rows; }
        }

        public void SetTile(int index, bool value) {
            TileData[index] = value;
        }

        public void SetTile(int x, int y, bool value) {
            SetTile(Util.OneDee(Columns, x, y), value);
        }

        public void ClearTile(int index) {
            SetTile(index, false);
        }

        public void ClearTile(int x, int y) {
            SetTile(x, y, false);
        }

        public bool GetTile(int index) {
            if (index < 0 || index >= TileCount) return false;
            return TileData[index];
        }

        public bool GetTile(int x, int y) {
            if (x < 0 || y < 0 || x >= Columns || y >= Columns) return false;
            return GetTile(Util.OneDee(Columns, x, y));
        }

        public bool GetRectGrid(int x, int y, int x2, int y2) {
            for (int i = x; i <= x2; i++) {
                for (int j = y; j <= y2; j++) {
                    if (GetTile(i, j))
                        return true;
                }
            }
            return false;
        }

        public bool GetRect(float x, float y, float x2, float y2) {
            var gridx = (int)Util.SnapToGrid(x, TileWidth) / TileWidth;
            var gridy = (int)Util.SnapToGrid(y, TileHeight) / TileHeight;
            var gridx2 = (int)Util.SnapToGrid(x2, TileWidth) / TileWidth;
            var gridy2 = (int)Util.SnapToGrid(y2, TileHeight) / TileHeight;
            return GetRectGrid(gridx, gridy, gridx2, gridy2);
        }
    }
}
