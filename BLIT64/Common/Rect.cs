
using System;

namespace BLIT64
{
    public readonly struct Rect : IEquatable<Rect>
    {
        public static ref Rect Empty => ref _empty;

        public readonly int X;
        public readonly int Y;
        public readonly int W;
        public readonly int H;

        public int Left => X;
        public int Top => Y;
        public int Right => X + W;
        public int Bottom => Y + H;

        private static Rect _empty = new Rect(0, 0, 0, 0);


        public Rect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }


        public bool IsEmpty => W == 0 && H == 0;

        public bool Contains(int x, int y)
        {
            return ((((X <= x) && (x < (X + W))) && (Y <= y)) && (y < (Y + H)));
        }

        public bool Contains(Rect other)
        {
            return ((((X <= other.X) && ((other.X + other.W) <= (X + W))) && (Y <= other.Y)) && ((Y +other.H) <= (Y + H)));
        }

        public bool Contains(int x, int y, int w, int h)
        {
            return ((((X <= x) && ((x + w) <= (X + W))) && (Y <= y)) && ((Y + h) <= (Y + H)));
        }

        public bool Intersects(int x, int y, int w, int h)
        {
            return x < Right &&
                   X < x + w &&
                   y < Bottom &&
                   Y < y + h;
        }
       
        public bool Intersects(Rect other)
        {
            return other.X < Right &&
                   X < other.Right &&
                   other.Y < Bottom &&
                   Y < other.Bottom;
        }

        public Rect Inflate(int delta)
        {
            return new Rect(X - delta, Y - delta, W + 2 * delta, H + 2 * delta);
        }

        public Rect Deflate(int delta)
        {
            return new Rect(X + delta, Y + delta, W - 2 * delta, H - 2 * delta);
        }

        public bool Equals(Rect other)
        {
            return (X == other.X) && (Y == other.Y) && (W == other.W) && (H == other.H);
        }

        public override bool Equals(object other)
        {
            return other is Rect rect && Equals(rect);
        }

        public override int GetHashCode()
        {
            return X + Y + W + H;
        }

        public static bool Equals(ref Rect value1, ref Rect value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.W == value2.W &&
                   value1.H == value2.H;
        }

        public static bool operator ==(Rect value1, Rect value2)
        {
            return Equals(ref value1, ref value2);
        }

        public static bool operator !=(Rect value1, Rect value2)
        {
            return !Equals(ref value1, ref value2);
        }

        public override string ToString()
        {
            return $"{{{X},{Y},{W},{H}}}";
        }
    }
}
