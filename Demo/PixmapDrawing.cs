using BLIT64;

namespace Demo
{
    public class PixmapDrawing : Scene
    {
        private Pixmap pixmap;

        public override void Load()
        {
            pixmap = Assets.CreatePixmap(256, 256);
            
            Canvas.SetTarget(pixmap);

            int half_pixmap = pixmap.Width/2;

            Canvas.SetColor(33);
            Canvas.RectFill(0, 0, pixmap.Width, pixmap.Height);
            Canvas.SetColor(20);
            Canvas.RectFill(0, 0, half_pixmap, half_pixmap);
            Canvas.RectFill(half_pixmap, half_pixmap, half_pixmap, half_pixmap);

            Canvas.SetTarget();
        }

        public override void Update()
        {
        }

        public override void Draw(Canvas canvas)
        {
            canvas.Pixmap(pixmap, x: Game.Canvas.Width / 2 - pixmap.Width / 2, y: Game.Canvas.Height / 2 - pixmap.Height / 2, src_rect: Rect.Empty);

            canvas.Pixmap(pixmap, x: 0, y: 0, src_rect: new Rect(0, 0, pixmap.Width/2, pixmap.Height/2));

            canvas.Pixmap(pixmap, x: Game.Canvas.Width - pixmap.Width/2, y: Game.Canvas.Height - pixmap.Height/2, src_rect: new Rect(pixmap.Width / 2, 0, pixmap.Width / 2, pixmap.Height / 2));

            canvas.Pixmap(pixmap, x: 25, y: Game.Canvas.Height - pixmap.Height/2 - 20, src_rect: Rect.Empty, width: pixmap.Width/2.0f, height: pixmap.Height/2.0f);

            canvas.Pixmap(pixmap, x: Game.Canvas.Width - pixmap.Width/2 - 20, y: 25, src_rect: Rect.Empty, width: pixmap.Width / 2.0f, height: pixmap.Height / 2.0f);

        }
    }
}
