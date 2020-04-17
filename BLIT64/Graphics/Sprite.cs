
namespace BLIT64
{
    internal struct Sprite
    {
        public int X;
        public int Y;
        public int Index;
        public int Width;
        public int Height;

        public Sprite( int index, int x, int y, int width, int height)
        {
            Index = index;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
