using BLIT64;

namespace Demo
{
    public class Rect
    {
        public int X;
        public int Y;
        public int W;
        public int H;
        public int Dx;
        public int Dy;

        public Rect(int x, int y, int w, int h, int dx, int dy)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
            Dx = dx;
            Dy = dy;
        }

        public void Update()
        {
            X += Dx;
            Y += Dy;

            if (X + W > 640)
            {
                Dx = -Dx;
                X = 640 - W;
            }
            else if (X < 0)
            {
                X = 0;
                Dx = -Dx;
            }

            if (Y + H > 360)
            {
                Dy = -Dy;
                Y = 360 - H;
            }
            else if (Y < 0)
            {
                Y = 0;
                Dy = -Dy;
            }
        }

        public void Draw(Blitter blitter)
        {
            blitter.Rect(X, Y, W, H, 9);
        }
    }
}
