using BLIT64;
using BLIT64.Toolkit.Gui;

namespace BLIT64_Editor
{
    public abstract class SpriteSheetDisplayComponent : Container
    {
        protected SpriteSheet CurrentSpritesheet;

        protected SpriteSheetDisplayComponent(string id, int width, int height) : base(id, width, height)
        {
        }

        public virtual void SetSpriteSheet(SpriteSheet sprite_sheet)
        {
            CurrentSpritesheet = sprite_sheet;
        }

        protected virtual (int X, int Y) GetTransformedMousePos(int x, int y)
        {
            var scale_factor = (float)Width / CurrentSpritesheet.Width;

            var sprite_sheet_surface_pos_x = Calc.FastFloorToInt(x / scale_factor);
            var sprite_sheet_surface_pos_y = Calc.FastFloorToInt(y / scale_factor);

            return (sprite_sheet_surface_pos_x, sprite_sheet_surface_pos_y);
        }

        
    }
}
