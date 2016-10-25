using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public struct Vertex {

        public float X;
        public float Y;
        public float U;
        public float V;
        public Color Color;

        public Vertex(float x, float y, float u, float v, Color color) {
            X = x;
            Y = y;
            U = u;
            V = v;
            Color = color;
        }

        public Vertex(float x, float y, Color color) : this(x, y, 0, 0, color) {

        }

        public Vertex(float x, float y) : this(x, y, 0, 0, 0xFFFFFFFF) {

        }

        public override string ToString() {
            return string.Format("X: {0} Y: {1} U: {2} V: {3} {4}", X, Y, U, V, Color);

        }
    }
}
