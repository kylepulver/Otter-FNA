using System;

namespace Otter {
    public class Text : Graphic {
        public Color Color = Color.White;

        public Font Font;

        public string String;

        public Text(Font font, string str) {
            Font = font;
            String = str;
        }

        public override void Render() {
            Draw.End();
            Draw.Begin(RenderPosition, Scale, Rotation, Origin);
            var penPosition = Vector2.Zero;
            for (int i = 0; i < String.Length; i++) {
                var c = String[i];
                
                if (c == ' ') {
                    penPosition.X += Font.AdvanceSpace;
                    continue;
                }
                if (c == '\n') {
                    penPosition.X = 0;
                    penPosition.Y += Font.LineHeight;
                    continue;
                }

                var glyph = Font.GetGlyph(c);
                var x = penPosition.X + glyph.BearingX;
                var y = penPosition.Y - glyph.BearingY;

                Draw.Texture(Font.GetGlyphTexture(c), Font.GetGlyphBounds(c), new Vector2(x, y), Color);

                penPosition.X += glyph.Advance;
                if (i < String.Length - 1) {
                    var nextC = String[i + 1];
                    penPosition.X += Font.GetKerning(c, nextC);
                }
            }
            Draw.End();
            Draw.Begin();
        }
    }
}
