using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Axis : Knob {

        float localX;
        float localY;

        float overrideX;
        float overrideY;

        public float X {
            get { return localX; }
            set { localX = value; }
        }
        public float Y {
            get { return localY; }
            set { localY = value; }
        }

        Dictionary<Direction, List<Key>> keys = new Dictionary<Direction, List<Key>>();

        public Vector2 Position {
            get { return new Vector2(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        public Button Up { get; private set; }
        public Button Left { get; private set; }
        public Button Down { get; private set; }
        public Button Right { get; private set; }

        public static Axis CreateWASD() {
            var axis = new Axis();
            axis.Add(Key.W, Key.A, Key.S, Key.D);
            return axis;
        }

        public static Axis CreateArrowKeys() {
            var axis = new Axis();
            axis.Add(Key.Up, Key.Left, Key.Down, Key.Right);
            return axis;
        }

        public Axis() {
            foreach (Direction d in Enum.GetValues(typeof(Direction))) {
                keys.Add(d, new List<Key>());
            }

            Up = new Button();
            Left = new Button();
            Down = new Button();
            Right = new Button();

            Up.IsUpdatedByInput = false;
            Left.IsUpdatedByInput = false;
            Down.IsUpdatedByInput = false;
            Right.IsUpdatedByInput = false;
        }

        public void Add(Direction direction, Key key) {
            keys[direction].Add(key);
        }

        public void Add(Key up, Key left, Key down, Key right) {
            keys[Direction.Up].Add(up);
            keys[Direction.Left].Add(left);
            keys[Direction.Down].Add(down);
            keys[Direction.Right].Add(right);
        }

        internal override void UpdateInput(Input input) {
            base.UpdateInput(input);

            X = 0;
            Y = 0;

            foreach (Key k in keys[Direction.Up])
                if (input.IsKeyDown(k))
                    Y -= 1;

            foreach (Key k in keys[Direction.Down])
                if (input.IsKeyDown(k))
                    Y += 1;

            foreach (Key k in keys[Direction.Left]) 
                if (input.IsKeyDown(k))
                    X -= 1;
            
            foreach (Key k in keys[Direction.Right]) 
                if (input.IsKeyDown(k))
                    X += 1;

            Up.UpdateInput(input);
            Left.UpdateInput(input);
            Down.UpdateInput(input);
            Right.UpdateInput(input);

            
        }
    }
}
