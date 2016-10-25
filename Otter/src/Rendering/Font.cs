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

        internal Texture FontTexture;

        static Library _lib = new Library();
        Dictionary<char, Glyph> _glyphs = new Dictionary<char, Glyph>();
        Dictionary<char, uint> _glyphIndex = new Dictionary<char, uint>();

        Dictionary<char, Dictionary<char, int>> _kerning = new Dictionary<char, Dictionary<char, int>>();

        //List<Glyph> _glyphs = new List<Glyph>();
        int charMin = 32;
        int charMax = 256;
        int textureSize = 1024;
        byte[] _glyphsBufferData;

        public int AdvanceSpace { get; private set; }
        public int LineHeight { get; private set; }

        public int Size { get; private set; }

        public Font(string path, int size) {
            var face = new Face(_lib, path);

            Size = size;
            face.SetPixelSizes(0, (uint)Size);
            LineHeight = (int)face.Size.Metrics.Height;

            face.LoadGlyph(face.GetCharIndex(32), LoadFlags.Default, LoadTarget.Normal);
            AdvanceSpace = (int)face.Glyph.Metrics.HorizontalAdvance;

            for (int i = charMin; i < charMax; i++) {
                var glyphIndex = face.GetCharIndex((uint)i);
                _glyphIndex.Add((char)i, glyphIndex);

                face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
                byte[] bufferData;
                if (face.Glyph.Metrics.Width == 0) {
                    bufferData = new byte[0];
                }
                else {
                    face.Glyph.RenderGlyph(RenderMode.Normal);
                    bufferData = face.Glyph.Bitmap.BufferData;
                }
                _glyphs.Add((char)i, new Glyph(i - charMin, face.Glyph.Metrics, bufferData));
                for (int j = charMin; j < charMax; j++) {
                    var otherGlyphIndex = face.GetCharIndex((uint)j);
                    var k = (int)face.GetKerning(glyphIndex, otherGlyphIndex, KerningMode.Default).X;
                    if (k != 0) {
                        if (!_kerning.ContainsKey((char)i))
                            _kerning.Add((char)i, new Dictionary<char, int>());
                        _kerning[(char)i].Add((char)j, k);
                    }
                }
            }

            var x = 0;
            var y = 0;

            _glyphsBufferData = new byte[textureSize * textureSize];

            foreach (var glyph in _glyphs) {
                var bmpData = glyph.Value.BufferData;

                for (int by = 0; by < glyph.Value.Height; by++) {
                    for (int bx = 0; bx < glyph.Value.Width; bx++) {
                        var src = by * glyph.Value.Width + bx;
                        var dest = (by + y) * textureSize + bx + x;
                        _glyphsBufferData[dest] = bmpData[src];

                    }
                }

                x += Size;
                if (x >= textureSize) {
                    x = 0;
                    y += Size;
                }
            }

            Resources.OnGraphicsReady((gd) => {
                var colors = new Microsoft.Xna.Framework.Color[textureSize * textureSize];

                var texture2d = new Texture2D(gd, textureSize, textureSize, false, SurfaceFormat.Color);
                for (int j = 0; j < textureSize; j++) {
                    for (int i = 0; i < textureSize; i++) {
                        var b = _glyphsBufferData[(j * textureSize) + i];
                        colors[(j * textureSize) + i] = new Microsoft.Xna.Framework.Color(255, 255, 255, b);
                    }
                }

                texture2d.SetData(colors);
                FontTexture = new Texture(texture2d);
            });
        }

        public Glyph GetGlyph(char glyph) {
            return _glyphs[glyph];
        }

        public float GetKerning(char left, char right) {
            if (!_kerning.ContainsKey(left)) return 0;
            if (!_kerning[left].ContainsKey(right)) return 0;
            return _kerning[left][right];
        }

        public Rectangle GetGlyphBounds(char glyph) {
            if (glyph == ' ') return Rectangle.Empty;
            
            var glyphId = _glyphs[glyph].Id;

            var width = textureSize / Size;
            var x = (glyphId % width) * Size;
            var y = glyphId / width * Size;
            var w = Size;
            var h = Size;

            return new Rectangle(x, y, w, h);
        }

        void Load() {
            
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
