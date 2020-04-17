using System;
using System.Globalization;

namespace BLIT64
{
    public readonly struct Color : IEquatable<Color>
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        public readonly byte A;

        private static Color _empty = new Color(0, 0, 0, 0);

        public static ref Color Empty => ref _empty; 

        public Color(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Color(uint color)
        {
            R = (byte)((color >> 16) & 0xFF);
            G = (byte)((color >> 8) & 0xFF);
            B = (byte)((color) & 0xFF);
            A = 255;
        }


        public override string ToString()
        {
            return $"R:{R}, G:{G}, B:{B}, A:{A}";
        }

        public override int GetHashCode()
        {
            return R + G + B + A;
        }

        public int Delta(ref Color other)
        {
            return Delta(other.R, other.G, other.B);
        }

        public int Delta(byte r, byte g, byte b)
        {
            return Math.Abs(R - r) + Math.Abs(G - g) + Math.Abs(B - b);
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        public static implicit operator Color(uint value)
        {
            var r = (byte)((value >> 24) & 0xFF);
            var g = (byte)((value >> 16) & 0xFF);
            var b = (byte)((value >> 8) & 0xFF);
            var a = (byte)(value & 0xFF);

            return new Color(r, g, b, a);
        }

        public static implicit operator Color(string hex)
        {
            uint value;
            try
            {
                var span = hex.AsSpan();
                if (span[0] == '#')
                {
                    span = span.Slice(1);
                }
                value = uint.Parse(span, NumberStyles.HexNumber);
                if (span.Length == 6)
                {
                    value = (value << 8) + 0xFF;
                }
            }
            catch
            {
                throw new ArgumentException($"Failed to parse the hex rgb '{hex}' as an unsigned 32-bit integer.");
            }

            return value;
        }

        public override bool Equals(object obj)
        {
            if (obj is Color color)
            {
                return Equals(color);
            }

            return false;
        }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }
    }
}
