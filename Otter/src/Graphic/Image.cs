﻿namespace Otter {
    public class Image : Graphic {
        public Color Color = Color.White;

        public Texture Texture;

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
            Draw.Texture(Texture, Texture.Bounds, RenderPosition, Scale, Rotation, Origin, Color, Shader);
        }
    }
}
