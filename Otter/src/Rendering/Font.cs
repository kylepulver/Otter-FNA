using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;
using System.IO;

namespace Otter {
    public class Font {

        public Texture FontTexture;

        public List<Texture> FontTextures = new List<Texture>();

        static Library lib = new Library();
        Dictionary<char, Glyph> glyphs = new Dictionary<char, Glyph>();
        Dictionary<char, uint> glyphIndex = new Dictionary<char, uint>();

        Dictionary<char, Dictionary<char, int>> kerning = new Dictionary<char, Dictionary<char, int>>();

        int charMin = 32;
        int charMax = 256;
        int textureSize = 2048;
        byte[] glyphsBufferData;

        public int AdvanceSpace { get; private set; }
        public int LineHeight { get; private set; }

        public int Size { get; private set; }

        public Font(string path, int size) {
            var face = new Face(lib, path);

            Size = size;
            face.SetPixelSizes(0, (uint)Size);
            LineHeight = (int)face.Size.Metrics.Height;

            face.LoadGlyph(face.GetCharIndex(32), LoadFlags.Default, LoadTarget.Normal);
            AdvanceSpace = (int)face.Glyph.Metrics.HorizontalAdvance;

            for (int i = charMin; i < charMax; i++) {
                var glyphIndex = face.GetCharIndex((uint)i);
                this.glyphIndex.Add((char)i, glyphIndex);

                face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
                byte[] bufferData;
                if (face.Glyph.Metrics.Width == 0) {
                    bufferData = new byte[0];
                }
                else {
                    face.Glyph.RenderGlyph(RenderMode.Normal);
                    bufferData = face.Glyph.Bitmap.BufferData;
                }
                glyphs.Add((char)i, new Glyph(i - charMin, face.Glyph.Metrics, bufferData));
                for (int j = charMin; j < charMax; j++) {
                    var otherGlyphIndex = face.GetCharIndex((uint)j);
                    var k = (int)face.GetKerning(glyphIndex, otherGlyphIndex, KerningMode.Default).X;
                    if (k != 0) {
                        if (!kerning.ContainsKey((char)i))
                            kerning.Add((char)i, new Dictionary<char, int>());
                        kerning[(char)i].Add((char)j, k);
                    }
                }
            }

            var x = 0;
            var y = 0;

            List<byte[]> glyphsBufferData = new List<byte[]>();

            glyphsBufferData.Add(new byte[textureSize * textureSize]);

            foreach (var glyph in glyphs) {
                var bmpData = glyph.Value.BufferData;

                for (int by = 0; by < glyph.Value.Height; by++) {
                    for (int bx = 0; bx < glyph.Value.Width; bx++) {
                        var src = by * glyph.Value.Width + bx;
                        var dest = (by + y) * textureSize + bx + x;
                        glyphsBufferData[glyphsBufferData.Count - 1][dest] = bmpData[src];
                    }
                }

                // Size + 1 to give glyphs padding preventing artifacts
                var s = Size + 1;
                x += s;
                if (x > textureSize - s) {
                    x = 0;
                    y += s;
                    if (y > textureSize - s) {
                        glyphsBufferData.Add(new byte[textureSize * textureSize]);
                        x = 0;
                        y = 0;
                    }
                }
            }

            Resources.OnGraphicsReady((gd) => {
                foreach(var data in glyphsBufferData) {
                    var colors = new Microsoft.Xna.Framework.Color[textureSize * textureSize];

                    var texture2d = new Texture2D(gd, textureSize, textureSize, false, SurfaceFormat.Color);
                    for (int j = 0; j < textureSize; j++) {
                        for (int i = 0; i < textureSize; i++) {
                            var b = data[(j * textureSize) + i];
                            colors[(j * textureSize) + i] = new Microsoft.Xna.Framework.Color(255, 255, 255, b);
                        }
                    }

                    texture2d.SetData(colors);
                    FontTextures.Add(new Texture(texture2d));
                    //FontTexture = new Texture(texture2d);
                }
            });
        }

        public Glyph GetGlyph(char glyph) {
            return glyphs[glyph];
        }

        public float GetKerning(char left, char right) {
            if (!kerning.ContainsKey(left)) return 0;
            if (!kerning[left].ContainsKey(right)) return 0;
            return kerning[left][right];
        }

        public Texture GetGlyphTexture(char glyph) {
            // todo: return real texture
            return FontTextures[0];
        }

        public Rectangle GetGlyphBounds(char glyph) {
            if (glyph == ' ') return Rectangle.Empty;
            
            var glyphId = glyphs[glyph].Id;

            // Size + 1 to give glyphs padding preventing artifacts
            var size = Size + 1;
            var width = textureSize / size;
            var x = (glyphId % width) * size;
            var y = glyphId / width * size;
            var w = size;
            var h = size;

            if (x + w >= textureSize || y + h >= textureSize) {
                Console.WriteLine("glyph {0} is outside of the texture ;_;", glyph);
            }

            return new Rectangle(x, y, w - 1, h - 1);
        }
    }

    public struct Glyph {
        public int Id;
        public int Width;
        public int Height;
        public int Advance;
        public int BearingX;
        public int BearingY;
        public char Character;
        public byte[] BufferData;

        public Glyph(int id, GlyphMetrics metrics, byte[] bufferData) {
            Id = id;
            Character = (char)id;

            Width = (int)metrics.Width;
            Height = (int)metrics.Height;
            Advance = (int)metrics.HorizontalAdvance;
            BearingX = (int)metrics.HorizontalBearingX;
            BearingY = (int)metrics.HorizontalBearingY;
            BufferData = bufferData;
        }
    }

    class BinPacker {
        public int Width { get; private set; }
        public int Height { get; private set; }

        List<Rectangle> nodes = new List<Rectangle>();

        public BinPacker() {
            nodes.Add(new Rectangle(0, 0, int.MaxValue, int.MaxValue));
        }

        public bool Pack(int w, int h, out Rectangle rect) {
            for (int i = 0; i < nodes.Count; ++i) {
                if (w <= nodes[i].Width && h <= nodes[i].Height) {
                    var node = nodes[i];
                    nodes.RemoveAt(i);
                    rect = new Rectangle(node.X, node.Y, w, h);
                    nodes.Add(new Rectangle(rect.Right, rect.Y, node.Right - rect.Right, rect.Height));
                    nodes.Add(new Rectangle(rect.X, rect.Bottom, rect.Width, node.Bottom - rect.Bottom));
                    nodes.Add(new Rectangle(rect.Right, rect.Bottom, node.Right - rect.Right, node.Bottom - rect.Bottom));

                    Width = Math.Max(Width, rect.Right);
                    Height = Math.Max(Height, rect.Bottom);

                    return true;
                }
            }
            rect = Rectangle.Empty;
            return false;
        }
    }
}
