
using BLIT64;

namespace BLIT64_Editor
{
    public class FillTool : Tool
    {
        public override void OnMouseDown(ToolActionParams @params)
        {
            var blitter = @params.Blitter;
            var x = @params.X;
            var y = @params.Y;
            var button = @params.MouseButton;
            var paint_color = @params.PaintColor;
            var sheet = @params.SpriteSheet;
            var source_rect = @params.SourceRect;

            blitter.SetSurface(sheet);
            blitter.Clip(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);
            blitter.FloodFill(x, y, button == MouseButton.Left ? paint_color : (byte)0);
            blitter.Clip();
            blitter.ResetSurface();
        }

        public override void OnMouseUp(ToolActionParams @params)
        {
        }

        public override void OnMouseMove(ToolActionParams @params)
        {
        }
    }
}
