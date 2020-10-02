using BLIT64;

namespace BLIT64_Editor
{
    public class PenTool : Tool
    {
        private bool _mouse_down = false;
        private int _last_x;
        private int _last_y;


        public override int BrushSize
        {
            get => _brush_size;
            set => _brush_size = value;
        }

        public override bool UseVariableBrushSize => true;


        public override void OnMouseDown(ToolActionParams @params)
        {
            _mouse_down = true;

            var snapped_x = Calc.SnapToInt(@params.PaintX, BrushSize);
            var snapped_y = Calc.SnapToInt(@params.PaintY, BrushSize);

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

                var snapped_x = Calc.SnapToInt(@params.PaintX, BrushSize);
                var snapped_y = Calc.SnapToInt(@params.PaintY, BrushSize);

                _last_x = snapped_x;
                _last_y = snapped_y;
            }
        }

        public override void OnKeyDown(Key key, ToolActionParams @params)
        {
        }

        public override void Update(ToolActionParams @params)
        {
        }

        private void PaintAt(ToolActionParams @params)
        {
            var blitter = @params.Blitter;
            var sheet = @params.SpriteSheet;
            var x = Calc.SnapToInt(@params.PaintX, BrushSize);
            var y = Calc.SnapToInt(@params.PaintY, BrushSize);
            var button = @params.MouseButton;
            var paint_color = button == MouseButton.Left ? @params.PaintColor : (byte)0;



            blitter.SetSurface(sheet);
            blitter.Clip(@params.SourceRect);

            if (_last_x != x || _last_y != y)
            {
                blitter.Line(_last_x, _last_y, x, y, BrushSize, paint_color);
            }
            else
            {
                blitter.Rect(x, y, _brush_size, _brush_size, paint_color);
            }

            
            blitter.Clip();
            blitter.SetSurface(null);
        }
    }
}
