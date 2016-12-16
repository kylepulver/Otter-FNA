using Microsoft.Xna.Framework.Graphics;
using System;

namespace Otter {
    public class Image : Graphic {
        public Color Color = Color.White;

        public Texture Texture;

        public bool FlipX;
        public bool FlipY;

        public static Image CreateRectangle(int size, Color color) {
            return CreateRectangle(size, size, color);
        }

        public static Image CreateRectangle(int size) {
            return CreateRectangle(size, Color.White);
        }

        public static Image CreateRectangle(int width, int height) {
            return CreateRectangle(width, height, Color.White);
        }

        public static Image CreateRectangle(int width, int height, Color color) {
            Image image = null;
            Resources.OnGraphicsReady((gd) => {
                var colors = new Microsoft.Xna.Framework.Color[width * height];
                var texture2d = new Texture2D(gd, width, height, false, SurfaceFormat.Color);
                for (int j = 0; j < texture2d.Bounds.Height; j++) {
                    for (int i = 0; i < texture2d.Bounds.Width; i++) {
                        colors[(j * texture2d.Bounds.Width) + i] = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, color.A);
                    }
                }
                texture2d.SetData(colors);
                image = new Image(new Texture(texture2d));
            });
            return image; // May return null if GD isn't ready :I
        }

        public static Image CreateCircle(int diameter) {
            return CreateCircle(diameter, Color.White);
        }

        public static Image CreateCircle(int diameter, Color color) {
            Image image = null;
            Resources.OnGraphicsReady((gd) => {
                var radius = diameter * 0.5f;
                var rsquared = radius * radius;
                var size = (int)(radius * 2);
                var colors = new Microsoft.Xna.Framework.Color[size * size];
                var texture2d = new Texture2D(gd, size, size, false, SurfaceFormat.Color);
                for (int j = 0; j < texture2d.Bounds.Height; j++) {
                    for (int i = 0; i < texture2d.Bounds.Width; i++) {
                        var x = i + 0.5f;
                        var y = j + 0.5f;

                        var a = 0f;
                        var distSquared = (radius - x) * (radius - x) + (radius - y) * (radius - y);
                        if (distSquared <= rsquared) {
                            a = 1f;
                        }

                        colors[(j * texture2d.Bounds.Width) + i] = new Microsoft.Xna.Framework.Color(color.R, color.G, color.B, a * color.A);
                    }
                }
                texture2d.SetData(colors);
                image = new Image(new Texture(texture2d));
            });
            return image; // May return null if GD isn't ready :I
        }

        public Image(string path) {
            Texture = new Texture(path);
            Initialize();
        }

        public Image(Texture texture) {
            Texture = texture;
            Initialize();
        }

        public override void Initialize() {
            base.Initialize();

            Resources.OnGraphicsReady((gd) => {
                UpdateData();
            });
        }

        protected override void UpdateData() {
            base.UpdateData();

            Width = Texture.Bounds.Width;
            Height = Texture.Bounds.Height;
        }

        public override void Render() {
            Draw.Texture(Texture, Texture.Bounds, RenderPosition, Scale, Rotation, Origin, Color, Shader, FlipX, FlipY);
        }
    }
}
