using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otter {
    public class Text : Graphic {

        public Font Font;

        public string String;

        public Text(Font font, string str) {
            Font = font;
            String = str;
        }

        public override void Render() {
            var penPosition = RenderPosition;
            for (int i = 0; i < String.Length; i++) {
                var c = String[i];
                
                if (c == ' ') {
                    penPosition.X += Font.AdvanceSpace;
                    continue;
                }
                if (c == '\n') {
                    penPosition.X = RenderPosition.X;
                    penPosition.Y += Font.LineHeight;
                    continue;
                }

                var glyph = Font.GetGlyph(c);
                var x = penPosition.X + glyph.BearingX;
                var y = penPosition.Y - glyph.BearingY;

                Draw.Texture(Font.FontTexture, Font.GetGlyphBounds(c), new Vector2(x, y), Color.White);

                penPosition.X += glyph.Advance;
                if (i < String.Length - 1) {
                    var nextC = String[i + 1];
                    penPosition.X += Font.GetKerning(c, nextC);
                }
            }
        }
    }
}
