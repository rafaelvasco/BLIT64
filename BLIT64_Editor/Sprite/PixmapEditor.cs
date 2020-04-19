using BLIT64;

namespace BLIT64_Editor
{
    internal class PixmapEditor : PixmapDisplayComponent
    {
        private const int CURSOR_BORDER_SIZE = 2;
        private const int PANEL_BORDER_SIZE = 2;

        public static readonly int Size = SpriteEditor.TileSize * 16;

        private int _last_paint_x;
        private int _last_paint_y;
        private int _brush_size = 1;
        private int _mouse_x;
        private int _mouse_y;
        private int _paint_color = 1;
        private bool _mouse_down;
        private bool _shift_down;
        private bool _draw_cursor;
        private MouseButton _mouse_button_down;

        public PixmapEditor(Blitter blitter, Rect area) : base(blitter, area)
        {
        }

        public void SetPaintColor(int color)
        {
            _paint_color = color;
        }

        public void SetBrushSize(int size)
        {
            _brush_size = size;
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_down = true;

            _mouse_button_down = button;

            var (paint_x, paint_y) = GetPaintPos(x, y);

            _last_paint_x = paint_x;
            _last_paint_y = paint_y;

            _blitter.SetSurface(_current_pixmap);
            if (_brush_size < 2)
            {
                _blitter.Pixel(_last_paint_x, _last_paint_y, button == MouseButton.Left ?_paint_color : -1);
            }
            else
            {
                var snapped_x = Calc.SnapToInt(_last_paint_x, _brush_size);
                var snapped_y = Calc.SnapToInt(_last_paint_y, _brush_size);
                _blitter.Rect(snapped_x, snapped_y, _brush_size, _brush_size, button == MouseButton.Left ?_paint_color : -1);
            }
            _blitter.ResetSurface();
        }

        public void ClearFrame()
        {
            _blitter.SetSurface(_current_pixmap);

            ref var source_rect = ref PixmapViewer.Instance.SourceRect;

            _blitter.Clip(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);

            _blitter.Clear(-1);

            _blitter.Clip();

            _blitter.ResetSurface();
        }

        public void ClearAll()
        {
            _blitter.SetSurface(_current_pixmap);

            _blitter.Clear(-1);

            _blitter.ResetSurface();
        }

        private (int X, int Y) GetPaintPos(int x, int y)
        {
            var (transformed_x, transformed_y) = GetLocalTransformedMousePos(x, y);

            ref var source_rect = ref PixmapViewer.Instance.SourceRect;

            return (transformed_x + source_rect.X, transformed_y + source_rect.Y);
        }

        protected override (int X, int Y) GetLocalTransformedMousePos(int x, int y)
        {
            ref var source_rect = ref PixmapViewer.Instance.SourceRect;

            var source_surface_factor_x = (float) source_rect.W / _current_pixmap.Width;
            var source_surface_factor_y = (float) source_rect.H / _current_pixmap.Height;

            var (transformed_x, tranformed_y) = base.GetLocalTransformedMousePos(x, y);

            return (Calc.FastFloorToInt(transformed_x * source_surface_factor_x),
                Calc.FastFloorToInt(tranformed_y * source_surface_factor_y));
        }

        public override void OnMouseEnter()
        {
            _draw_cursor = true;
        }

        public override void OnMouseLeave()
        {
            _draw_cursor = false;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _mouse_down = false;
        }

        public override void OnMouseMove(int x, int y)
        {
            _mouse_x = x;
            _mouse_y = y;

            if (!_mouse_down)
            {
                return;
            }

            ref var source_rect = ref PixmapViewer.Instance.SourceRect;

            var (paint_x, paint_y) = GetPaintPos(x, y);

            if (
                paint_x < source_rect.X || 
                paint_y < source_rect.Y || 
                paint_x > (source_rect.X + source_rect.W - _brush_size) || 
                paint_y > (source_rect.Y + source_rect.H - _brush_size)
            )
            {
                return;
            }

            var delta_x = paint_x - _last_paint_x;
            var delta_y = paint_y - _last_paint_y;

            if (delta_x != 0 || delta_y != 0)
            {
                _blitter.SetSurface(_current_pixmap);

                if (_brush_size < 2)
                {
                    _blitter.Pixel(paint_x, paint_y, _mouse_button_down == MouseButton.Left ?_paint_color : -1);
                }
                else
                {
                    var snapped_x = Calc.SnapToInt(paint_x, _brush_size);
                    var snapped_y = Calc.SnapToInt(paint_y, _brush_size);
                    _blitter.Rect(snapped_x, snapped_y, _brush_size, _brush_size, _mouse_button_down == MouseButton.Left ?_paint_color : -1);
                }

                _blitter.ResetSurface();

                _last_paint_x = paint_x;
                _last_paint_y = paint_y;
            }
        }

        public override void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.C:
                    if (!_shift_down)
                    {
                        ClearFrame();
                    }
                    else
                    {
                        ClearAll();
                    }
                    break;
                case Key.LeftShift:
                    _shift_down = true;
                    break;

                case Key.D1:
                    SetBrushSize(1);
                    break;
                case Key.D2:
                    SetBrushSize(2);
                    break;
                case Key.D3:
                    SetBrushSize(4);
                    break;
            }
        }

        public override void OnKeyUp(Key key)
        {
            switch (key)
            {
                case Key.LeftShift:
                    _shift_down = false;
                    break;
            }
        }


        public override void Draw()
        {
            var blitter = _blitter;

            ref var source_rect = ref PixmapViewer.Instance.SourceRect;

            blitter.Pixmap(_current_pixmap, _area.X, _area.Y, source_rect, _area.W, _area.H);

            var scale_factor = (float)_area.W / _current_pixmap.Width;

            var source_surface_factor_x = _current_pixmap.Width / source_rect.W;
            var source_surface_factor_y = _current_pixmap.Height / source_rect.H;

            var final_scale_x = (int) scale_factor * source_surface_factor_x;
            var final_scale_y = (int) scale_factor * source_surface_factor_y;

            var cursor_x = (int)(_area.X + Calc.Snap(_mouse_x, _brush_size * final_scale_x));
            var cursor_y = (int)(_area.Y + Calc.Snap(_mouse_y, _brush_size * final_scale_y));

            if (_draw_cursor)
            {
                blitter.RectBorder(
                    cursor_x + CURSOR_BORDER_SIZE, 
                    cursor_y + CURSOR_BORDER_SIZE, 
                    (_brush_size * final_scale_x) - CURSOR_BORDER_SIZE*2, 
                    (_brush_size * final_scale_y) - CURSOR_BORDER_SIZE*2, 
                    CURSOR_BORDER_SIZE, 
                    0);

                blitter.RectBorder(cursor_x, cursor_y,  _brush_size * final_scale_x, _brush_size * final_scale_y, CURSOR_BORDER_SIZE);
            }

            blitter.RectBorder(
                _area.X,
                _area.Y,
                _area.W,
                _area.H,
                PANEL_BORDER_SIZE
            );

            blitter.Rect(
                x: _area.X - PANEL_BORDER_SIZE, 
                y: _area.Y + _area.H + PANEL_BORDER_SIZE, 
                w: _area.W + 2 * PANEL_BORDER_SIZE, 
                h: PANEL_BORDER_SIZE, 
                col_index: 0);

            var sprite_id_txt = $"#{PixmapViewer.Instance.CurrentSpriteId:000}";

            var text_measure = blitter.TextMeasure(sprite_id_txt, 2);

            blitter.Text(_area.X + _area.W/2 - text_measure.Width/2, _area.Y - text_measure.Height - 5, sprite_id_txt, 2);
        }
    }
}
