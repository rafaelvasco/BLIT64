using BLIT64;

namespace Demo
{
    public class Scene1 : Scene
    {
        private Rect[] rects;
        private Pixmap pixmap;
        private const int count = 10;
        public int X = 0;
        public int Y = 0;

        public override void Load()
        {
            rects = new Rect[count];

            for (int i = 0; i < rects.Length; ++i)
            {
                rects[i] = new Rect(100, 100, 20, 20, ((count - i)),((count - i)));
            }

            pixmap = new Pixmap(16, 16);

            Blitter.SetSurface(pixmap);

            Blitter.Rect(0, 0, 16, 16, Palette.WhiteColor);
            Blitter.Rect(0, 0, 8, 8, Palette.WhiteColor);
            Blitter.Rect(8, 8, 8, 8, Palette.WhiteColor);

            Blitter.SetSurface(null);


        }

        public override void Update()
        {
            for (int i = 0; i < rects.Length; ++i)
            {
                rects[i].Update();
            }

            if (Input.KeyPressed(Key.Space))
            {
                Game.SetPalette(Game.CurrentPalette == Palettes.Journey ? Palettes.Famicube : Palettes.Journey);
            }

            if (Input.KeyDown(Key.Left))
            {
                X -= 2;
            }

            if (Input.KeyDown(Key.Right))
            {
                X += 2;
            }

            if (Input.KeyDown(Key.Up))
            {
                Y -= 2;
            }

            if (Input.KeyDown(Key.Down))
            {
                Y += 2;
            }

        }

        public override void Draw(Blitter blitter)
        {
            var clip_rect = new BLIT64.Rect(0, 0, Game.Width, Game.Height).Deflate(20);

            blitter.Clip(clip_rect.X, clip_rect.Y, clip_rect.W, clip_rect.H);

            blitter.Clear(33);
            for (int i = 0; i < rects.Length; ++i)
            {
                rects[i].Draw(blitter);
            }

            blitter.RectBorder(X - 50, Y - 50, 100, 100, 2);

            blitter.Pixel(X, Y, 35);

            blitter.Pixmap(pixmap, X-32, Y-32, BLIT64.Rect.Empty, 64, 64);

            blitter.Text(50, 50, $"Mouse Pos: {Input.MousePos}");

            blitter.Line(0, 0, Game.Width, Game.Height, 2, Palette.WhiteColor);
            blitter.Line(0, Game.Height, Game.Width, 0, 2, Palette.WhiteColor);
            blitter.Line(Game.Width/2, 0, Game.Width/2, Game.Height, 2, Palette.WhiteColor);
            blitter.Line(0, Game.Height/2, Game.Width, Game.Height/2, 2, Palette.WhiteColor);
            blitter.Line(0, 0, Input.MousePos.X, Input.MousePos.Y, 1, Palette.WhiteColor);
        }
    }
}
