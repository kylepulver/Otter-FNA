using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Button : Knob {

        List<Key> keys = new List<Key>();
        List<MouseButton> mouseButtons = new List<MouseButton>();
        List<MouseWheelDirection> directions = new List<MouseWheelDirection>();
        List<List<int>> controllerButtons = new List<List<int>>();

        public Button() {
            
        }

        public void Add(Key key) {
            keys.Add(key);
        }

        public void Add(MouseButton mouseButton) {
            mouseButtons.Add(mouseButton);
        }

        public void Add(MouseWheelDirection direction) {
            directions.Add(direction);
        }

        public void Add(int controllerId, int buttonId) {

        }

        public bool IsDown {
            get { return currentDown; }
        }

        public bool IsUp {
            get { return !IsDown; }
        }

        public bool IsPressed {
            get { return IsDown && !previousDown; }
        }

        public bool IsReleased {
            get { return IsUp && previousDown; }
        }

        bool currentDown;
        bool previousDown;

        internal override void UpdateInput(Input input) {
            base.UpdateInput(input);

            previousDown = currentDown;
            currentDown = false;

            foreach (var k in keys) {
                if (input.IsKeyDown(k)) {
                    currentDown = true;
                    break;
                }
            }

            if (!currentDown) {
                foreach (var mb in mouseButtons) {
                    if (input.IsMouseButtonDown(mb)) {
                        currentDown = true;
                        break;
                    }
                }
            }

            if(!currentDown) {
                foreach(var d in directions) {
                    if (input.MouseWheelDelta == (int)d) {
                        currentDown = true;
                        break;
                    }
                    if (d == MouseWheelDirection.Any) {
                        currentDown = input.MouseWheelDelta != 0;
                        break;
                    }
                }
            }
        }
    }
}
