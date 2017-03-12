using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Otter {
    public struct Color : IEquatable<Color> {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color(float r, float g, float b) : this(r, g, b, 1) { }

        public Color(float r, float g, float b, float a) {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(byte r, byte g, byte b, byte a) : this() {
            ByteR = r;
            ByteG = g;
            ByteB = b;
            ByteA = a;
        }

        public Color(Color color) : this(color.R, color.G, color.B, color.A) { }

        public Color(UInt32 color) {
            R = (byte)(color >> 24) / 255f;
            G = (byte)(color >> 16) / 255f;
            B = (byte)(color >> 8) / 255f;
            A = (byte)(color >> 0) / 255f;
        }

        public byte ByteR {
            set { R = Convert.ToSingle(value) / 255f; }
            get { return (byte)(R * 255); }
        }
        public byte ByteG {
            set { G = Convert.ToSingle(value) / 255f; }
            get { return (byte)(G * 255); }
        }
        public byte ByteB {
            set { B = Convert.ToSingle(value) / 255f; }
            get { return (byte)(B * 255); }
        }
        public byte ByteA {
            set { A = Convert.ToSingle(value) / 255f; }
            get { return (byte)(A * 255); }
        }

        public static Color White { get { return new Color(1, 1, 1); } }
        public static Color Black { get { return new Color(0, 0, 0); } }
        public static Color Red { get { return new Color(1, 0, 0); } }
        public static Color Blue { get { return new Color(0, 0, 1); } }
        public static Color Green { get { return new Color(0, 1, 0); } }
        public static Color Yellow { get { return new Color(1, 1, 0); } }
        public static Color None { get { return new Color(0, 0, 0, 0); } }
        public static Color Magenta { get { return new Color(1, 0, 1); } }
        public static Color Grey { get { return new Color(0.5f, 0.5f, 0.5f); } }
        public static Color Cyan { get { return new Color(0, 1, 1); } }
        public static Color Random { get { return Rand.Color; } }
        public static Color RandomAlpha { get { return Rand.ColorAlpha; } }

        public static Color Shade(float light) { return new Color(light, light, light); }

        public static implicit operator UInt32(Color color) {
            return (uint)((color.ByteR << 24) | (color.ByteG << 16) | (color.ByteB << 8) | (color.ByteA << 0));
        }

        public static implicit operator Color(UInt32 value) {
            return new Color(value);
        }

        public static implicit operator Color(Microsoft.Xna.Framework.Color color) {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static implicit operator Color(string value) {
            int red = 255, green = 255, blue = 255, alpha = 255;

            if (value.Length == 6) {
                red = Convert.ToInt32(value.Substring(0, 2), 16);
                green = Convert.ToInt32(value.Substring(2, 2), 16);
                blue = Convert.ToInt32(value.Substring(4, 2), 16);
            }
            else if (value.Length == 3) {
                red = Convert.ToInt32(value.Substring(0, 1) + value.Substring(0, 1), 16);
                green = Convert.ToInt32(value.Substring(1, 1) + value.Substring(1, 1), 16);
                blue = Convert.ToInt32(value.Substring(2, 1) + value.Substring(2, 1), 16);
            }
            else if (value.Length == 2) {
                red = Convert.ToInt32(value.Substring(0, 1) + value.Substring(0, 1), 16);
                green = Convert.ToInt32(value.Substring(0, 1) + value.Substring(0, 1), 16);
                blue = Convert.ToInt32(value.Substring(0, 1) + value.Substring(0, 1), 16);
                alpha = Convert.ToInt32(value.Substring(1, 2) + value.Substring(1, 2), 16);
            }
            else if (value.Length == 8) {
                red = Convert.ToInt32(value.Substring(0, 2), 16);
                green = Convert.ToInt32(value.Substring(2, 2), 16);
                blue = Convert.ToInt32(value.Substring(4, 2), 16);
                alpha = Convert.ToInt32(value.Substring(6, 2), 16);
            }
            else if (value.Length == 4) {
                red = Convert.ToInt32(value.Substring(0, 1) + value.Substring(0, 1), 16);
                green = Convert.ToInt32(value.Substring(1, 1) + value.Substring(1, 1), 16);
                blue = Convert.ToInt32(value.Substring(2, 1) + value.Substring(2, 1), 16);
                alpha = Convert.ToInt32(value.Substring(3, 1) + value.Substring(2, 1), 16);
            }

            var r = Util.ScaleClamp(red, 0, 255, 0, 1);
            var g = Util.ScaleClamp(green, 0, 255, 0, 1);
            var b = Util.ScaleClamp(blue, 0, 255, 0, 1);
            var a = Util.ScaleClamp(alpha, 0, 255, 0, 1);

            return new Color(r, g, b, a);
        }

        public bool Equals(Color other) {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override string ToString() {
            return string.Format("R:{0} G:{1} B:{2} A:{3}", R, G, B, A);
        }

        public static Color operator *(Color value1, Color value2) {
            return new Color(value1.R * value2.R, value1.G * value2.G, value1.B * value2.B, value1.A * value2.A);
        }

        public static Color operator *(Color value1, float value2) {
            return new Color(value1.R * value2, value1.G * value2, value1.B * value2);
        }

        public static Color operator +(Color value1, Color value2) {
            return new Color(value1.R + value2.R, value1.G + value2.G, value1.B + value2.B, value1.A + value2.A);
        }

        public static Color operator -(Color value1, Color value2) {
            return new Color(value1.R - value2.R, value1.G - value2.G, value1.B - value2.B, value1.A - value2.A);
        }

        public static Color operator /(Color value1, Color value2) {
            return new Color(value1.R / value2.R, value1.G / value2.G, value1.B / value2.B, value1.A / value2.A);
        }
    }
}
