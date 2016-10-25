using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    static class XnaExtensions {

        internal static Microsoft.Xna.Framework.Graphics.Texture2D ToXnaTexture(this Texture texture) {
            return texture.XnaTexture;
        }

        internal static Microsoft.Xna.Framework.Graphics.Effect ToXnaEffect(this Shader shader) {
            return shader.XnaEffect;
        }

        internal static Microsoft.Xna.Framework.Color ToXnaColor(this Color color) {
            return new Microsoft.Xna.Framework.Color(color.ByteR, color.ByteG, color.ByteB, color.ByteA);
        }

        internal static Microsoft.Xna.Framework.Rectangle ToXnaRectangle(this Rectangle rect) {
            return new Microsoft.Xna.Framework.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        internal static Microsoft.Xna.Framework.Vector3 ToXnaVector3(this Vector3 v) {
            return new Microsoft.Xna.Framework.Vector3(v.X, v.Y, v.Z);
        }

        internal static Microsoft.Xna.Framework.Vector2 ToXnaVector2(this Vector2 v) {
            return new Microsoft.Xna.Framework.Vector2(v.X, v.Y);
        }

        internal static Microsoft.Xna.Framework.Vector4 ToXnaVector4(this Vector4 v) {
            return new Microsoft.Xna.Framework.Vector4(v.X, v.Y, v.Z, v.W);
        }

        internal static Microsoft.Xna.Framework.Matrix ToXnaMatrix(this Matrix m) {
            return new Microsoft.Xna.Framework.Matrix(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
        }

        internal static Microsoft.Xna.Framework.Graphics.VertexPositionColor ToXnaVertex(this Vertex v) {
            return new Microsoft.Xna.Framework.Graphics.VertexPositionColor(new Vector3(v.X, v.Y, 0).ToXnaVector3(), v.Color.ToXnaColor());
        }

        internal static Microsoft.Xna.Framework.Graphics.VertexPositionColor[] ToXnaVertices(this Vertex[] v) {
            var v2 = new Microsoft.Xna.Framework.Graphics.VertexPositionColor[v.Length];
            for (int i = 0; i < v.Length; i++) {
                v2[i] = v[i].ToXnaVertex();
            }
            return v2;
        }

        internal static Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture ToXnaVertexTexture(this Vertex v) {
            return new Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture(new Vector3(v.X, v.Y, 0).ToXnaVector3(), v.Color.ToXnaColor(), new Vector2(v.U, v.V).ToXnaVector2());
        }

        internal static Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[] ToXnaVerticesTexture(this Vertex[] v) {
            var v2 = new Microsoft.Xna.Framework.Graphics.VertexPositionColorTexture[v.Length];
            for (int i = 0; i < v.Length; i++) {
                v2[i] = v[i].ToXnaVertexTexture();
            }
            return v2;
        }
    }
}
