using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Knob : Component {

        public bool IsUpdatedByInput = true;

        public Knob() {
            Game.Instance.WhenReady(() => {
                Game.Instance.Input.Knobs.Add(this);
                Console.WriteLine("knob added");
            });
        }

        internal virtual void UpdateInput(Input input) {
        }
    }
}
