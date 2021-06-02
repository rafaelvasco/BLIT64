using BLIT64;

namespace Demo
{
    public class Scene2 : Scene
    {
        private int x = 320;
        private int y = 180;
        private int w = 6;
        private int h = 6;
        private int dx = 7;
        private int dy = 4;
        private byte col;


        public override void Load()
        {
        }

        public override void Update()
        {
            x += dx;
            y += dy;

            if (x > 640 - w || x < 0)
            {
                dx = -dx;
                col += 1;

                if (col > 63)
                {
                    col = 0;
                }
            }

            if (y > 360 - h || y < 0)
            {
                dy = -dy;
            }

            if (Input.KeyPressed(Key.Space))
            {
                
            }
        }

        public override void Draw(Canvas canvas)
        {
            canvas.SetColor(col);
            canvas.RectFill(x, y, w, h);
        }
    }
}
