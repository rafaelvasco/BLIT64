
namespace BLIT64
{
    public class DashedRect : Graphic
    {
        public byte Color { get; set; } = Palette.WhiteColor;

        public int AnimDelay { get; set; } = 3;

        public int LineSize { get; set; } = 1;

        public int DashSize { get; set; } = 3;

        private int _dash_offset;
        private int _animation_timer;
        

        public override void Draw(Canvas blitter, int x, int y, int w, int h)
        {
            blitter.SetColor(Color);
            blitter.RectDashed(
                x,
                y,
                w,
                h,
                LineSize,
                _dash_offset,
                DashSize
            );

        }

        public override void Update()
        {
            _animation_timer += 1;

            if (_animation_timer > AnimDelay)
            {
                _animation_timer = 0;

                _dash_offset += 1;

                if (_dash_offset > 2)
                {
                    _dash_offset = 0;
                }
            }
        }
    }
}
