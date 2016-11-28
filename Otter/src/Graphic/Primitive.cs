using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Primitive : Graphic {

        public Vertex[] Vertices;
        public Vertex[] VerticesOutline;

        Color _color;
        public Color Color {
            get { return _color; }
            set {
                if (_color != value) {
                    _color = value;
                    NeedsUpdate = true;
                }
            }
        }

        float _outlineWidth;
        public float OutlineWidth {
            get { return _outlineWidth; }
            set {
                if (_outlineWidth != value) {
                    _outlineWidth = value;
                    NeedsUpdate = true;
                }
            }
        }

        Color _outlineColor;
        public Color OutlineColor {
            get { return _outlineColor; }
            set {
                if (_outlineColor != value) {
                    _outlineColor = value;
                    NeedsUpdate = true;
                }
            }
        }

        public static int CirclePointCount = 32;

        Action<float, Color> UpdateOutline = delegate { };

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        int[] indices;
        BasicEffect basicEffect;

        VertexBuffer vertexBufferOutline;
        IndexBuffer indexBufferOutline;
        int[] indicesOutline;

        public Primitive() {
            Resources.OnGraphicsReady((gd) => {
                basicEffect = new BasicEffect(gd);
                basicEffect.VertexColorEnabled = true;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter
                   (0, gd.Viewport.Width,     // left, right
                    gd.Viewport.Height, 0,    // bottom, top
                    0, 1).ToXnaMatrix();                    // near, far plane
            });
        }

        public static Primitive CreateCircle(float radius, Color color) {
            var p = new Primitive();

            var center = new Vector2(radius, radius);

            p.Color = color;
            p.Vertices = new Vertex[CirclePointCount + 1];

            p.Vertices[0] = new Vertex(center.X, center.Y, color);

            for (int i = 1; i <= CirclePointCount; i++) {
                var angle = 360f / CirclePointCount * (i - 1);
                var v = Vector2.CreateFromAngleLength(angle, radius);
                p.Vertices[i] = new Vertex(v.X + center.X, v.Y + center.Y, color);
            }

            p.indices = new int[CirclePointCount * 3];

            var n = 1;
            for (int i = 0; i < p.indices.Length; i += 3) {
                p.indices[i] = n + 1;
                p.indices[i + 1] = n;
                p.indices[i + 2] = 0;
                if (n + 1 > CirclePointCount) {
                    p.indices[i] = 1;
                }
                n++;
            }

            Resources.OnGraphicsReady((gd) => {
                p.vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColor), p.Vertices.Length, BufferUsage.WriteOnly);
                p.vertexBuffer.SetData(p.Vertices.ToXnaVertices());

                p.indexBuffer = new IndexBuffer(gd, typeof(int), p.indices.Length, BufferUsage.WriteOnly);
                p.indexBuffer.SetData(p.indices);
            });

            p.UpdateOutline += (outlineWidth, outlineColor) => {
                p.VerticesOutline = new Vertex[CirclePointCount * 2];

                for (int i = 0; i < CirclePointCount; i++) {
                    var angle = 360f / CirclePointCount * i;
                    var v = Vector2.CreateFromAngleLength(angle, radius);
                    p.VerticesOutline[i] = new Vertex(v.X + center.X, v.Y + center.Y, outlineColor);
                }

                for (int i = 0; i < CirclePointCount; i++) {
                    var angle = 360f / CirclePointCount * i;
                    var v = Vector2.CreateFromAngleLength(angle, radius + outlineWidth);
                    p.VerticesOutline[i + CirclePointCount] = new Vertex(v.X + center.X, v.Y + center.Y, outlineColor);
                }
                
                p.indicesOutline = new int[CirclePointCount * 3 * 2];

                n = 0;
                for (int i = 0; i < p.indicesOutline.Length; i+=6) {
                    if (n == CirclePointCount - 1) {
                        p.indicesOutline[i] = 0;
                        p.indicesOutline[i + 1] = CirclePointCount;
                        p.indicesOutline[i + 2] = CirclePointCount - 1;
                    }
                    else {
                        p.indicesOutline[i] = n + 1;
                        p.indicesOutline[i + 1] = n + 1 + CirclePointCount;
                        p.indicesOutline[i + 2] = n + CirclePointCount;
                    }

                    p.indicesOutline[i + 3] = n + CirclePointCount;
                    p.indicesOutline[i + 4] = n;
                    p.indicesOutline[i + 5] = n + 1;
                    n++;
                }
                             
                Resources.OnGraphicsReady((gd) => {
                    if (p.vertexBufferOutline != null)
                        p.vertexBufferOutline.Dispose();    
                    p.vertexBufferOutline = new VertexBuffer(gd, typeof(VertexPositionColor), p.VerticesOutline.Length, BufferUsage.WriteOnly);
                    p.vertexBufferOutline.SetData(p.VerticesOutline.ToXnaVertices());

                    // memory explosion here:
                    if (p.indexBufferOutline != null)
                        p.indexBufferOutline.Dispose();
                    p.indexBufferOutline = new IndexBuffer(gd, typeof(int), p.indicesOutline.Length, BufferUsage.WriteOnly);
                    p.indexBufferOutline.SetData(p.indicesOutline);
                });

            };

            p.UpdateData();
            return p;
        }

        public static Primitive CreateRectangle(float width, float height, Color color) {
            var p = new Primitive();
            p.Color = color;
            p.Vertices = new Vertex[4];

            p.Vertices[0] = new Vertex(0, 0, Color.White);
            p.Vertices[1] = new Vertex(width, 0, Color.White);
            p.Vertices[2] = new Vertex(width, height, Color.White);
            p.Vertices[3] = new Vertex(0, height, Color.White);

            p.indices = new int[] { 0, 1, 2, 2, 3, 0 };

            Resources.OnGraphicsReady((gd) => {
                p.vertexBuffer = new VertexBuffer(gd, typeof(VertexPositionColor), p.Vertices.Length, BufferUsage.WriteOnly);
                p.vertexBuffer.SetData(p.Vertices.ToXnaVertices());
                
                p.indexBuffer = new IndexBuffer(gd, typeof(int), p.indices.Length, BufferUsage.WriteOnly);
                p.indexBuffer.SetData(p.indices);
            });

            p.UpdateOutline += (outlineWidth, outlineColor) => {
                p.VerticesOutline = new Vertex[8];

                p.VerticesOutline[0] = new Vertex(0, 0, outlineColor);
                p.VerticesOutline[1] = new Vertex(width, 0, outlineColor);
                p.VerticesOutline[2] = new Vertex(width, height, outlineColor);
                p.VerticesOutline[3] = new Vertex(0, height, outlineColor);

                p.VerticesOutline[4] = new Vertex(-outlineWidth, -outlineWidth, outlineColor);
                p.VerticesOutline[5] = new Vertex(width + outlineWidth, -outlineWidth, outlineColor);
                p.VerticesOutline[6] = new Vertex(width + outlineWidth, height + outlineWidth, outlineColor);
                p.VerticesOutline[7] = new Vertex(-outlineWidth, height + outlineWidth, outlineColor);

                p.indicesOutline = new int[] {
                    0, 4, 5,
                    1, 0, 5,
                    1, 5, 6,
                    2, 1, 6,
                    2, 6, 7,
                    3, 2, 7,
                    3, 7, 4,
                    0, 3, 4
                };

                Resources.OnGraphicsReady((gd) => {
                    p.vertexBufferOutline = new VertexBuffer(gd, typeof(VertexPositionColor), p.VerticesOutline.Length, BufferUsage.WriteOnly);
                    p.vertexBufferOutline.SetData(p.VerticesOutline.ToXnaVertices());

                    p.indexBufferOutline = new IndexBuffer(gd, typeof(int), p.indicesOutline.Length, BufferUsage.WriteOnly);
                    p.indexBufferOutline.SetData(p.indicesOutline);
                });
            };

            p.UpdateData();
            return p;
        }

        public static Primitive CreateRectangle(float width, float height) {
            return CreateRectangle(width, height, Color.White);
        }

        protected override void UpdateData() {
 	        base.UpdateData();

            Width = Vertices.Max(v => v.X) - Vertices.Min(v => v.X);
            Height = Vertices.Max(v => v.Y) - Vertices.Min(v => v.Y);

            for (int i = 0; i < Vertices.Length; i++) {
                Vertices[i].Color = Color;
            }
            if (vertexBuffer != null) {
                vertexBuffer.SetData(Vertices.ToXnaVertices());
            }

            UpdateOutline(OutlineWidth, OutlineColor);
        }

        public override void Render() {
            base.Render();

            var gd = Core.Instance.GraphicsDevice;

            for (int i = 0; i < basicEffect.CurrentTechnique.Passes.Count; i++) {

                basicEffect.View = Draw.GetTransformMatrix(RenderPosition, Scale, Rotation, Origin).ToXnaMatrix();
                basicEffect.View *= Draw.TargetSurface.GetCameraTransform().ToXnaMatrix();
                basicEffect.CurrentTechnique.Passes[i].Apply();

                gd.Indices = indexBuffer;
                gd.SetVertexBuffer(vertexBuffer);
                gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, Vertices.Length, 0, indices.Length / 3);

                if (OutlineWidth > 0) {
                    gd.Indices = indexBufferOutline;
                    gd.SetVertexBuffer(vertexBufferOutline);
                    gd.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, VerticesOutline.Length, 0, indicesOutline.Length / 3);
                }
            }
            
        }
    }
}
