using BLIT64;

namespace Demo
{
    

    public class Scene1 : Scene
    {
        private Rect[] rects;
        private Pixmap pixmap;
        private Pixmap loaded_pixmap;
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

            Blitter.Rect(0, 0, 16, 16, 0);
            Blitter.Rect(0, 0, 8, 8, 35);
            Blitter.Rect(8, 8, 8, 8, 35);

            Blitter.ResetSurface();

            loaded_pixmap = Assets.Get<Pixmap>("party");

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

            blitter.RectBorder(X - 50, Y - 50, 100, 100, 2, 35);

            blitter.Pixel(X, Y, 35);

            blitter.Pixmap(pixmap, X-32, Y-32, BLIT64.Rect.Empty, 64, 64);

            blitter.Pixmap(loaded_pixmap, 300, 150, BLIT64.Rect.Empty, -1, -1, -1, true);

            blitter.Line(0, 0, Game.Width, Game.Height, 2, 35);
            blitter.Line(0, Game.Height, Game.Width, 0, 2, 35);
            blitter.Line(Game.Width/2, 0, Game.Width/2, Game.Height, 2, 35);
            blitter.Line(0, Game.Height/2, Game.Width, Game.Height/2, 2, 35);
        }
    }
}
