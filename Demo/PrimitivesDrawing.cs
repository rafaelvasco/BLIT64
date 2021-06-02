using BLIT64;

namespace Demo
{
    public class PrimitivesDrawing : Scene
    {
        private Pixmap pixmap;

        public override void Load()
        {
            pixmap = Assets.CreatePixmap(200, 200);

            Canvas.SetSurface(pixmap);

            Canvas.SetColor(33);
            Canvas.RectFill(0, 0, 200, 200);
            Canvas.SetColor(20);
            Canvas.RectFill(0, 0, 100, 100);
            Canvas.RectFill(100, 100, 100, 100);

            Canvas.SetSurface(null);

        }

        public override void Update()
        {

        }

        public override void Draw(Canvas blitter)
        {
            var clip_rect = new Rect(0, 0, Game.Canvas.Width, Game.Canvas.Height).Deflate(20);

            blitter.Clip(clip_rect.X, clip_rect.Y, clip_rect.W, clip_rect.H);

            blitter.Clear();

            blitter.SetColor(20);

            blitter.Rect(70, 70, 100, 100, 1);
            blitter.Rect(50, 50, 140, 140, 2);
            blitter.Rect(30, 30, 180, 180, 4);

            blitter.Line(250, 50, 330, 210, 1);
            blitter.Line(280, 50, 370, 210, 2);
            blitter.Line(311, 50, 410, 210, 4);
            blitter.Line(341, 50, 450, 210, 8);

            blitter.Circle(550, 150, 100);

            blitter.RectFill(475, 267, 200, 200);

            blitter.DitherPatternBigCheckerBoard2();
            blitter.DitherColor(33);

            blitter.CircleFill(150, 350, 100);

            blitter.DitherPattern();

            blitter.Triangle(272, 310, 351, 230, 430, 331, 4);

        }
    }
}
