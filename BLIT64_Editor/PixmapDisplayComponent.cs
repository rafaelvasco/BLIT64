using BLIT64;

namespace BLIT64_Editor
{
    public abstract class PixmapDisplayComponent : Component
    {
        protected Pixmap _current_pixmap;

        protected PixmapDisplayComponent(Blitter blitter, Rect area) : base(blitter, area)
        {
        }

        public void SetPixmap(Pixmap pixmap)
        {
            _current_pixmap = pixmap;
        }

        protected virtual (int X, int Y) GetLocalTransformedMousePos(int x, int y)
        {
            var scale_factor = (float)_area.W / _current_pixmap.Width;

            var sprite_sheet_surface_pos_x = Calc.FastFloorToInt(x / scale_factor);
            var sprite_sheet_surface_pos_y = Calc.FastFloorToInt(y / scale_factor);

            return (sprite_sheet_surface_pos_x, sprite_sheet_surface_pos_y);
        }
    }
}
