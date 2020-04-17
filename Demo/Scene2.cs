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
        private int col;


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
                Game.SetPalette(Game.CurrentPalette == Palettes.Journey ? Palettes.Famicube : Palettes.Journey);
            }
        }

        public override void Draw(Blitter blitter)
        {
            blitter.Rect(x, y, w, h, col);
        }
    }
}
