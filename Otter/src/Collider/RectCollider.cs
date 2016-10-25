using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class RectCollider : Collider {

        public RectCollider(float width, float height) {
            Width = width;
            Height = height;
        }
    }
}
