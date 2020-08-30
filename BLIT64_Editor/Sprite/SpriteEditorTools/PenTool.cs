using BLIT64;

namespace BLIT64_Editor
{
    public class PenTool : Tool
    {
        private bool _mouse_down = false;
        private int _last_x;
        private int _last_y;

        public override void OnMouseDown(ToolActionParams @params)
        {
            _mouse_down = true;

            var snapped_x = Calc.SnapToInt(@params.X, @params.BrushSize);
            var snapped_y = Calc.SnapToInt(@params.Y, @params.BrushSize);

            _last_x = snapped_x;
            _last_y = snapped_y;

            PaintAt(@params);
        }

        public override void OnMouseUp(ToolActionParams @params)
        {
            _mouse_down = false;
        }

        public override void OnMouseMove(ToolActionParams @params)
        {
            if (_mouse_down)
            {
                PaintAt(@params);

                var snapped_x = Calc.SnapToInt(@params.X, @params.BrushSize);
                var snapped_y = Calc.SnapToInt(@params.Y, @params.BrushSize);

                _last_x = snapped_x;
                _last_y = snapped_y;
            }
        }

        private void PaintAt(ToolActionParams @params)
        {
            var blitter = @params.Blitter;
            var sheet = @params.SpriteSheet;
            var brush_size = @params.BrushSize;
            var x = Calc.SnapToInt(@params.X, brush_size);
            var y = Calc.SnapToInt(@params.Y, brush_size);
            var button = @params.MouseButton;
            var paint_color = button == MouseButton.Left ? @params.PaintColor : (byte)0;

            blitter.SetSurface(sheet);

            blitter.Line(_last_x, _last_y, x, y, brush_size, paint_color);
           
            blitter.ResetSurface();
        }
    }
}
