using BLIT64;

namespace Demo
{
    public class PrimitivesDrawing : Scene
    {
        private Pixmap pixmap;

        public override void Load()
        {
            pixmap = Assets.CreatePixmap(200, 200);

            Canvas.SetTarget(pixmap);

            Canvas.SetColor(33);
            Canvas.RectFill(0, 0, 200, 200);
            Canvas.SetColor(20);
            Canvas.RectFill(0, 0, 100, 100);
            Canvas.RectFill(100, 100, 100, 100);

            Canvas.SetTarget();

            var clip_rect = new Rect(0, 0, Game.Canvas.Width, Game.Canvas.Height).Deflate(20);

            Canvas.Clip(clip_rect.X, clip_rect.Y, clip_rect.W, clip_rect.H);

            Canvas.Clear();

            Canvas.SetColor(20);

            Canvas.Rect(70, 70, 100, 100, 1);
            Canvas.Rect(50, 50, 140, 140, 2);
            Canvas.Rect(30, 30, 180, 180, 4);

            Canvas.Line(250, 50, 330, 210, 1);
            Canvas.Line(280, 50, 370, 210, 2);
            Canvas.Line(311, 50, 410, 210, 4);
            Canvas.Line(341, 50, 450, 210, 8);

            Canvas.Circle(550, 150, 100);

            Canvas.RectFill(475, 267, 200, 200);

            Canvas.DitherPatternBigCheckerBoard2();
            Canvas.SetDitherColor(33);

            Canvas.CircleFill(150, 350, 100);

            Canvas.DitherPattern();

            Canvas.Triangle(272, 310, 351, 230, 430, 331, 4);

        }

        public override void Update()
        {
        }

        public override void Draw(Canvas canvas)
        {
        }
    }
}
