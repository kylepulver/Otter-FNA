using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    class Controller : Knob {

        public List<Button> Buttons;
        public List<Axis> Axes;

        internal override void UpdateInput(Input input) {
            base.UpdateInput(input);
            foreach(var a in Axes) {
                a.UpdateInput(input);
            }
            foreach (var b in Buttons) {
                b.UpdateInput(input);
            }
        }
    }
}
