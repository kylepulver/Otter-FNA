using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public struct Bounds {

        public float X;
        public float Y;
        public float Width;
        public float Height;

        public float Top {
            get { return Y; }
        }

        public float Left {
            get { return X; }
        }

        public float Bottom {
            get { return Y + Height; }
        }

        public float Right {
            get { return X + Width; }
        }

        public Bounds(float x, float y, float width, float height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
