using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class CircleCollider : Collider {
        public float Radius;

        public CircleCollider(float radius) {
            Radius = radius;

            Width = radius * 2;
            Height = radius * 2;
        }

        public override void Update() {
            base.Update();
            Width = Radius * 2;
            Height = Radius * 2;
        }
    }
}
