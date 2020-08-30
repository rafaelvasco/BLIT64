using System.Collections.Generic;
using BLIT64;

namespace BLIT64_Editor
{
    internal class SpriteSheetEditor : SpriteSheetDisplayComponent
    {
        public delegate void ColorPickEvent(byte color);

        public event ColorPickEvent OnColorPick = delegate {};

        private int _last_mouse_down_x;
        private int _last_mouse_down_y;
        private int _brush_size;
        private int _mouse_x;
        private int _mouse_y;
        private int _current_rotation_multipler = 0;
        private byte _paint_color = 1;
        private bool _shift_down;
        private bool _draw_cursor;
        private readonly SpriteEditorLayout _layout;
        private MouseButton _mouse_button_down;
        private readonly Dictionary<int, Tool> _tools;
        private Tool _current_tool;
        private readonly ToolActionParams _tool_action_params;



        public SpriteSheetEditor(SpriteEditorLayout layout, Blitter blitter, Rect area) : base(blitter, area)
        {
            _layout = layout;
            _tool_action_params = new ToolActionParams()
            {
                Blitter = _blitter,
            };

            _tools = new Dictionary<int, Tool>
            {
                {(int) Tools.Pen, new PenTool()},
                {(int) Tools.Select, new SelectTool()},
                {(int) Tools.Fill, new FillTool()}

            };

            SetCurrentTool(Tools.Pen);
            SetBrushSize(1);
        }

        public void SetCurrentTool(Tools tool)
        {
            if (_tools.TryGetValue((int)tool, out _))
            {
                _current_tool = _tools[(int)tool];
            }
        }

        public void SetPaintColor(byte color)
        {
            _paint_color = color;
            _tool_action_params.PaintColor = _paint_color;
        }

        public void SetBrushSize(int size)
        {
            _brush_size = size;
            _tool_action_params.BrushSize = size;
        }

        public override void SetSpriteSheet(SpriteSheet sprite_sheet)
        {
            base.SetSpriteSheet(sprite_sheet);
            _tool_action_params.SpriteSheet = sprite_sheet;
        }

        public void ClearFrame()
        {
            _blitter.SetSurface(CurrentSpritesheet);

            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;

            _blitter.Clip(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);

            _blitter.Clear();

            _blitter.Clip();

            _blitter.ResetSurface();
        }

        public void ClearAll()
        {
            _blitter.SetSurface(CurrentSpritesheet);

            _blitter.Clear();

            _blitter.ResetSurface();
        }

        public void Rotate()
        {
            _current_rotation_multipler += 1;

            if (_current_rotation_multipler > 3)
            {
                _current_rotation_multipler = 0;
            }
        }

        public void FlipH()
        {
            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;
            _blitter.SetSurface(CurrentSpritesheet);

            _blitter.FlipH(
                source_rect.X,
                source_rect.Y,
                source_rect.W,
                source_rect.H    
            );

            _blitter.ResetSurface();

        }

        public void FlipV()
        {
            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;
            _blitter.SetSurface(CurrentSpritesheet);

            _blitter.FlipV(
                source_rect.X,
                source_rect.Y,
                source_rect.W,
                source_rect.H    
            );

            _blitter.ResetSurface();
        }

        private (int X, int Y) GetPaintPos(int x, int y)
        {
            var (transformed_x, transformed_y) = GetTransformedMousePos(x, y);

            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;

            return (transformed_x + source_rect.X, transformed_y + source_rect.Y);
        }

        protected override (int X, int Y) GetTransformedMousePos(int x, int y)
        {
            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;

            var source_surface_factor_x = (float) source_rect.W / CurrentSpritesheet.Width;
            var source_surface_factor_y = (float) source_rect.H / CurrentSpritesheet.Height;

            var (transformed_x, tranformed_y) = base.GetTransformedMousePos(x, y);

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

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_button_down = button;

            var (paint_x, paint_y) = GetPaintPos(x, y);

            _last_mouse_down_x = paint_x;
            _last_mouse_down_y = paint_y;

            switch (button)
            {
                case MouseButton.Left:
                case MouseButton.Right:
                    _tool_action_params.X = paint_x;
                    _tool_action_params.Y = paint_y;
                    _tool_action_params.MouseButton = button;
                    _current_tool.OnMouseDown(_tool_action_params);
                    break;
                case MouseButton.Middle:
                    PickColorAt(paint_x, paint_y);
                    break;
            }
        }


        private void PickColorAt(int x, int y)
        {
            var pick_x = x;
            var pick_y = y;

            if (_brush_size > 1)
            {
                pick_x = Calc.SnapToInt(pick_x, _brush_size);
                pick_y = Calc.SnapToInt(pick_y, _brush_size);
            }

            OnColorPick.Invoke(CurrentSpritesheet.GetColorAt(pick_x, pick_y));
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            var (paint_x, paint_y) = GetPaintPos(x, y);

            _tool_action_params.X = paint_x;
            _tool_action_params.Y = paint_y;
            _tool_action_params.MouseButton = button;
            _current_tool.OnMouseUp(_tool_action_params);
            _mouse_button_down = MouseButton.None;
        }

        public override void OnMouseMove(int x, int y)
        {
            _mouse_x = x;
            _mouse_y = y;

            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;

            _tool_action_params.SourceRect = source_rect;

            var (paint_x, paint_y) = GetPaintPos(x, y);

            _tool_action_params.X = paint_x;
            _tool_action_params.Y = paint_y;

            _tool_action_params.MouseButton = _mouse_button_down;

            if (
                paint_x < source_rect.X || 
                paint_y < source_rect.Y || 
                paint_x > (source_rect.X + source_rect.W - _brush_size) || 
                paint_y > (source_rect.Y + source_rect.H - _brush_size)
            )
            {
                return;
            }

            var delta_x = paint_x - _last_mouse_down_x;
            var delta_y = paint_y - _last_mouse_down_y;

            if (delta_x != 0 || delta_y != 0)
            {
                switch (_mouse_button_down)
                {
                    case MouseButton.Left:
                    case MouseButton.Right:
                        _current_tool.OnMouseMove(_tool_action_params);
                        break;
                    case MouseButton.Middle:
                        if (_mouse_button_down != MouseButton.None)
                        {
                            PickColorAt(paint_x, paint_y);
                        }
                        
                        break;
                }


                _last_mouse_down_x = paint_x;
                _last_mouse_down_y = paint_y;
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

            ref var source_rect = ref SpriteSheetViewer.Instance.SourceRect;

            blitter.Rect(_area.X, _area.Y, _area.W, _area.H, 2);

            blitter.Pixmap(CurrentSpritesheet, _area.X, _area.Y, ref source_rect, _area.W, _area.H);

            var scale_factor = (float)_area.W / CurrentSpritesheet.Width;

            var source_surface_factor_x = CurrentSpritesheet.Width / source_rect.W;
            var source_surface_factor_y = CurrentSpritesheet.Height / source_rect.H;

            var final_scale_x = (int) scale_factor * source_surface_factor_x;
            var final_scale_y = (int) scale_factor * source_surface_factor_y;

            var cursor_x = (int)(_area.X + Calc.Snap(_mouse_x, _brush_size * final_scale_x));
            var cursor_y = (int)(_area.Y + Calc.Snap(_mouse_y, _brush_size * final_scale_y));

            var cursor_border_size = _layout.EditorCursorBorder;
            var panel_border_size = _layout.EditorPanelBorder;

            if (_draw_cursor)
            {
                blitter.RectBorder(
                    cursor_x + cursor_border_size, 
                    cursor_y + cursor_border_size, 
                    (_brush_size * final_scale_x) - cursor_border_size*2, 
                    (_brush_size * final_scale_y) - cursor_border_size*2, 
                    Palette.WhiteColor,
                    cursor_border_size);

                blitter.RectBorder(cursor_x, cursor_y,  _brush_size * final_scale_x, _brush_size * final_scale_y, Palette.WhiteColor, cursor_border_size);
            }

            blitter.RectBorder(
                _area.X,
                _area.Y,
                _area.W,
                _area.H,
                Palette.WhiteColor,
                panel_border_size
            );

            blitter.Rect(
                x: _area.X - panel_border_size, 
                y: _area.Y + _area.H + panel_border_size, 
                w: _area.W + 2 * panel_border_size, 
                h: panel_border_size, 
                color: Palette.BlackColor);

            var sprite_id_txt = $"#{SpriteSheetViewer.Instance.CurrentSpriteId:000}";

            var (text_width, text_height) = blitter.TextMeasure(sprite_id_txt, 2);

            blitter.Text(_area.X + _area.W/2 - text_width/2, _area.Y - text_height - 3, sprite_id_txt, 2, 2);
            blitter.Text(_area.X + _area.W/2 - text_width/2, _area.Y - text_height - 5, sprite_id_txt, 2);
            
        }
    }
}
