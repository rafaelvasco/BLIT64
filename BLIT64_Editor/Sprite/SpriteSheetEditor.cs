using System;
using System.Collections.Generic;
using BLIT64;
using BLIT64.Toolkit.Gui;
using BLIT64_Editor.Common;

namespace BLIT64_Editor
{
    internal class SpriteSheetEditor : SpriteSheetDisplayComponent
    {
        public Tool CurrentTool { get; private set; }

        private int _last_mouse_down_x;
        private int _last_mouse_down_y;
        private int _mouse_x;
        private int _mouse_y;
        private bool _mouse_inside;

        private byte _paint_color = 1;

        private bool _shift_down;
        private bool _selection_surface_filled;

        private int _select_width;
        private int _select_height;
        private int _select_translate_x;
        private int _select_translate_y;
        private int _select_src_x;
        private int _select_src_y;

        private Rect _sprite_source_rect;

        private MouseButton _mouse_button_down;

        private readonly Dictionary<int, Tool> _tools;
        private readonly ToolActionParams _tool_action_params;
        private readonly Pixmap _overlay_surface;
        private readonly Pixmap _select_surface;
        private readonly DashedRect _dashed_rect;
        private readonly Canvas _canvas;

        private AppLayout.LayoutData _layout = AppLayout.Data;


        public SpriteSheetEditor(string id, int width, int height) : base(id, width, height)
        {
            _canvas = Game.Instance.Canvas;

            _tool_action_params = new ToolActionParams()
            {
                Blitter = Game.Instance.Canvas,
            };

            _tools = new Dictionary<int, Tool>
            {
                {(int) Tools.Pen, new PenTool()},
                {(int) Tools.Select, new SelectTool()},
                {(int) Tools.Fill, new FillTool()}

            };

            SetCurrentTool(Tools.Pen);
            SetBrushSize(CurrentTool.BrushSize);

            _sprite_source_rect = Rect.Empty;

            _overlay_surface = Assets.CreatePixmap(Width, Height);
            _select_surface = Assets.CreatePixmap(Width, Height);

            _tool_action_params.Overlay = _overlay_surface;

            TypedMessager<Rect>.On(MessageCodes.SelectionRectResize, OnSelectRectChanged );
            TypedMessager<Rect>.On(MessageCodes.SelectionRectMoved, OnSelectionRectMoved);
            TypedMessager<Rect>.On(MessageCodes.SourceRectChanged, OnNavigatorSourceRectChanged);

            TypedMessager<byte>.On(MessageCodes.ColorSelected, SetPaintColor);
            TypedMessager<int>.On(MessageCodes.SpriteSheetEditorBrushSizeChanged, SetBrushSize);

            Messager.On(MessageCodes.SelectionStartedMoving, OnSelectionRectStartedMoving);
            _dashed_rect = new DashedRect();
        }

       


        public void SetCurrentTool(Tools tool)
        {
            CurrentTool?.OnDeactivate();

            if (_tools.TryGetValue((int)tool, out _))
            {
                CurrentTool = _tools[(int)tool];
                CurrentTool.OnActivate();
            }
        }

        public void SetPaintColor(byte color)
        {
            _paint_color = color;
            _tool_action_params.PaintColor = _paint_color;
        }

        public void SetBrushSize(int size)
        {
            if (CurrentTool.UseVariableBrushSize)
            {
                CurrentTool.BrushSize = size;
            }
        }

        public override void SetSpriteSheet(SpriteSheet sprite_sheet)
        {
            base.SetSpriteSheet(sprite_sheet);
            _tool_action_params.SpriteSheet = sprite_sheet;
        }

        public void ClearFrame()
        {
            _canvas.SetSurface(CurrentSpritesheet);

            var source_rect = GetGlobalModifyRegion();

            _canvas.Clip(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);

            _canvas.Clear();

            _canvas.Clip();

            _canvas.SetSurface(null);
        }

        public void ClearAll()
        {
            _canvas.SetSurface(CurrentSpritesheet);

            _canvas.Clear();

            _canvas.SetSurface(null);
        }

        public void Rotate()
        {
            var global_source_rect = GetGlobalModifyRegion();

            _canvas.SetSurface(CurrentSpritesheet);

            if (!SelectionEmpty())
            {
                if (!_selection_surface_filled)
                {
                    CutToSelectSurface(global_source_rect);
                }

                RotateSelectionRect();

                var target_rect = GetGlobalSelectionRect();

                _canvas.SetSurface(_select_surface);

                _canvas.Rotate90(
                    global_source_rect.X, 
                    global_source_rect.Y ,
                    global_source_rect.W, 
                    global_source_rect.H,
                    target_rect.X,
                    target_rect.Y,
                    target_rect.W,
                    target_rect.H);

                _canvas.SetSurface(null);
            }
            else
            {
                _canvas.Rotate90(
                    global_source_rect.X, 
                    global_source_rect.Y ,
                    global_source_rect.W, 
                    global_source_rect.H);
            }
           

            _canvas.SetSurface(null);
            
        }

        private void RotateSelectionRect()
        {

            int center_x = _select_src_x + _select_width / 2;
            int center_y = _select_src_y + _select_height / 2;

            int new_x = _select_src_x;
            int new_y = _select_src_y;
            int new_w = _select_height;
            int new_h = _select_width;

            var clamp_rect = _sprite_source_rect;

            new_x -= (_select_src_x + new_w / 2) - center_x;
            new_y -= (_select_src_y + new_h / 2) - center_y;

            if (new_x < 0)
            {
                new_x = 0;
            }

            if (new_y < 0)
            {
                new_y = 0;
            }

            if (new_x + new_w >= clamp_rect.W)
            {
                new_x -= new_x + new_w - clamp_rect.W;

            }

            if (new_y + new_h >= clamp_rect.H)
            {
                new_y -= new_y + new_h - clamp_rect.H;

            }

            _select_src_x = new_x;
            _select_src_y = new_y;
            _select_width = new_w;
            _select_height = new_h;
        }

        public void CutToSelectSurface(Rect region)
        {
            _canvas.SetSurface(_select_surface);

            _canvas.Pixmap(CurrentSpritesheet, region.X, region.Y, region);

            _canvas.SetSurface(null);

            _canvas.SetSurface(CurrentSpritesheet);

            _canvas.Clip(region);

            _canvas.Clear();

            _canvas.Clip();

            _canvas.SetSurface(null);

            _selection_surface_filled = true;
        }

        public void PasteFromSelectSurface()
        {
            if (!_selection_surface_filled)
            {
                return;
            }

            var global_select_rect = GetGlobalSelectionRect();
            var (global_x, global_y) = LocalFramePointToSpriteSheetPoint(_select_translate_x, _select_translate_y);

            _canvas.SetSurface(CurrentSpritesheet);

            _canvas.Pixmap(_select_surface, global_x, global_y, global_select_rect);

            _canvas.SetSurface(null);

            _canvas.SetSurface(_select_surface);

            _canvas.Clear();

            _canvas.SetSurface(null);

            _selection_surface_filled = false;

        }

        public void FlipH()
        {
            var global_source_rect = GetGlobalModifyRegion();

            if (SelectionEmpty())
            {
                _canvas.SetSurface(CurrentSpritesheet);

                _canvas.FlipH(
                    global_source_rect.X,
                    global_source_rect.Y,
                    global_source_rect.W,
                    global_source_rect.H
                );

                _canvas.SetSurface(null);
            }
            else
            {
                if (!_selection_surface_filled)
                {
                    CutToSelectSurface(global_source_rect);
                }


                _canvas.SetSurface(_select_surface);

                _canvas.FlipH(
                    global_source_rect.X,
                    global_source_rect.Y,
                    global_source_rect.W,
                    global_source_rect.H
                );

                _canvas.SetSurface(null);
                
            }
            
        }

        public void FlipV()
        {
            var global_source_rect = GetGlobalModifyRegion();

            if (SelectionEmpty())
            {
                _canvas.SetSurface(CurrentSpritesheet);

                _canvas.FlipV(
                    global_source_rect.X,
                    global_source_rect.Y,
                    global_source_rect.W,
                    global_source_rect.H
                );

                _canvas.SetSurface(null);
            }
            else
            {
                if (!_selection_surface_filled)
                {
                    CutToSelectSurface(global_source_rect);
                }


                _canvas.SetSurface(_select_surface);

                _canvas.FlipV(
                    global_source_rect.X,
                    global_source_rect.Y,
                    global_source_rect.W,
                    global_source_rect.H
                );

                _canvas.SetSurface(null);
                
            }
        }

        private (int PaintX, int PaintY, int EditorX, int EditorY) GetMousePositions(int x, int y)
        {
            var (transformed_x, transformed_y) = GetTransformedMousePos(x, y);

            var source_rect = _sprite_source_rect;

            return (transformed_x + source_rect.X, transformed_y + source_rect.Y, transformed_x, transformed_y);
        }

        protected override (int X, int Y) GetTransformedMousePos(int x, int y)
        {
            var source_rect = _sprite_source_rect;

            var source_surface_factor_x = (float) source_rect.W / CurrentSpritesheet.Width;
            var source_surface_factor_y = (float) source_rect.H / CurrentSpritesheet.Height;

            var (transformed_x, tranformed_y) = base.GetTransformedMousePos(x, y);

            return (Calc.FastFloorToInt(transformed_x * source_surface_factor_x),
                Calc.FastFloorToInt(tranformed_y * source_surface_factor_y));
        }

        public override void OnMouserEnter()
        {
            _mouse_inside = true;
        }


        public override void OnMouseExit()
        {
            _tool_action_params.PaintX = 0;
            _tool_action_params.PaintY = 0;
            _mouse_inside = false;
        }


        public override void Update()
        {
            CurrentTool.Update(_tool_action_params);

            if (!SelectionEmpty())
            {
               _dashed_rect.Update();
            }
        }


        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_button_down = button;

            var (paint_x, paint_y, editor_x, editor_y) = GetMousePositions(x, y);

            _last_mouse_down_x = paint_x;
            _last_mouse_down_y = paint_y;

            var source_rect = _sprite_source_rect;

            _tool_action_params.SourceRect = new Rect(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);

            switch (button)
            {
                case MouseButton.Left:
                case MouseButton.Right:
                    _tool_action_params.PaintX = paint_x;
                    _tool_action_params.PaintY = paint_y;
                    _tool_action_params.CursorX = editor_x;
                    _tool_action_params.CursorY = editor_y;
                    _tool_action_params.MouseButton = button;
                    CurrentTool.OnMouseDown(_tool_action_params);
                    break;
                case MouseButton.Middle:
                    PickColorAt(paint_x, paint_y);
                    break;
            }
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            var (paint_x, paint_y, editor_x, editor_y) = GetMousePositions(x, y);

            _tool_action_params.PaintX = paint_x;
            _tool_action_params.PaintY = paint_y;
            _tool_action_params.CursorX = editor_x;
            _tool_action_params.CursorY = editor_y;
            _tool_action_params.MouseButton = button;
            CurrentTool.OnMouseUp(_tool_action_params);
            _mouse_button_down = MouseButton.None;
        }

        public override void OnMouseMove(int x, int y)
        {

            _mouse_x = x;
            _mouse_y = y;

            var source_rect = _sprite_source_rect;

            _tool_action_params.SourceRect = new Rect(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);

            var (paint_x, paint_y, editor_x, editor_y) = GetMousePositions(x, y);

            _tool_action_params.PaintX = paint_x;
            _tool_action_params.PaintY = paint_y;
            _tool_action_params.CursorX = editor_x;
            _tool_action_params.CursorY = editor_y;

            _tool_action_params.MouseButton = _mouse_button_down;

            var delta_x = paint_x - _last_mouse_down_x;
            var delta_y = paint_y - _last_mouse_down_y;

            if (delta_x != 0 || delta_y != 0)
            {
                switch (_mouse_button_down)
                {
                    case MouseButton.Left:
                    case MouseButton.Right:
                        CurrentTool.OnMouseMove(_tool_action_params);
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
            CurrentTool.OnKeyDown(key, _tool_action_params);

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

        public override void Draw(Canvas blitter, IGuiDrawer drawer)
        {
            var source_rect = _sprite_source_rect;
            var scale_factor = (float)Width / CurrentSpritesheet.Width;
            var source_surface_factor = CurrentSpritesheet.Width / source_rect.W;
            var final_scale = (int) scale_factor * source_surface_factor;
            var brush_size = CurrentTool.BrushSize;
            var cursor_x = (int)(DrawX + Calc.Snap(_mouse_x, brush_size * final_scale));
            var cursor_y = (int)(DrawY + Calc.Snap(_mouse_y, brush_size * final_scale));
            var cursor_border_size = _layout.EditorCursorBorder;
            var panel_border_size = _layout.EditorPanelBorder;

            DrawSpriteSheet();

            DrawBorders();
           
            /* ========================================= */
            /* SELECT MODE */
            /* ========================================= */

            if (!SelectionEmpty())
            {
                DrawSelectElements();
            }

            DrawOverlay();

            if (_mouse_inside && CurrentTool.ShowCursor)
            {
                DrawCursor();
            }
            
            /* ========================================= */
            /* DRAW ELEMENTS IMPLEMENTATION */
            /* ========================================= */

            void DrawSpriteSheet()
            {
                blitter.SetColor(2);
                blitter.RectFill(DrawX, DrawY, Width, Height);

                blitter.Pixmap(CurrentSpritesheet, DrawX, DrawY, source_rect, Width, Height);
            }

            void DrawOverlay()
            {
                blitter.Pixmap(_overlay_surface, DrawX, DrawY, Rect.Empty);
                blitter.SetColor(Palette.WhiteColor);
                blitter.Text(DrawX + _layout.EditorMousePosLabelOffsetX, DrawY + _layout.EditorMousePosLabelOffsetY, $"{_tool_action_params.PaintX},{_tool_action_params.PaintY}");
            }

            void DrawSelectElements()
            {
                blitter.SetSurface(_overlay_surface);

                blitter.Clear();

                if (_selection_surface_filled)
                {
                    var selection_source_rect = GetGlobalSelectionRect();

                    blitter.Pixmap(_select_surface,
                        (_select_translate_x) * final_scale,
                        (_select_translate_y) * final_scale,
                        selection_source_rect,
                        _select_width * final_scale,
                        _select_height * final_scale);
                }

                _dashed_rect.Draw(
                    blitter, 
                    _select_translate_x * final_scale, 
                    _select_translate_y * final_scale, 
                    _select_width * final_scale, 
                    _select_height * final_scale
                );

                blitter.SetSurface(null);

            }

            void DrawCursor()
            {
                blitter.SetColor(Palette.BlackColor);
                blitter.Rect(
                    (cursor_x + cursor_border_size), 
                    (cursor_y + cursor_border_size), 
                    ((brush_size * final_scale) - cursor_border_size*2), 
                    ((brush_size * final_scale) - cursor_border_size*2), 
                    cursor_border_size);

                blitter.SetColor(Palette.WhiteColor);
                blitter.Rect(cursor_x, cursor_y,  (brush_size * final_scale), (brush_size * final_scale), cursor_border_size);
            }

            void DrawBorders()
            {
                blitter.SetColor(Palette.WhiteColor);
                blitter.Rect(
                    DrawX,
                    DrawY,
                    Width,
                    Height,
                    panel_border_size
                );

                blitter.SetColor(Palette.BlackColor);
                blitter.RectFill(
                    x: DrawX - panel_border_size,
                    y: DrawY + Height + panel_border_size,
                    w: Width + panel_border_size * 2,
                    h: panel_border_size);
            }
        }

        private void ClearSelectionRect()
        {
            _canvas.SetSurface(_overlay_surface);
    
            _canvas.Clear();

            _canvas.SetSurface(null);

            if (_selection_surface_filled)
            {
                PasteFromSelectSurface();
            }

            _select_src_x = 0;
            _select_src_y = 0;
            _select_width = 0;
            _select_height = 0;
            _select_translate_x = 0;
            _select_translate_y = 0;

        }

        private void OnSelectRectChanged(Rect obj)
        {
            if (obj.IsEmpty)
            {
                ClearSelectionRect();
            }
            else
            {
                _select_src_x = obj.X;
                _select_src_y = obj.Y;
                _select_width = obj.W;
                _select_height = obj.H;

                _select_translate_x = _select_src_x;
                _select_translate_y = _select_src_y;
            }
        }

        private void OnSelectionRectStartedMoving()
        {
            var global_selection_rect = GetGlobalSelectionRect();

            if (!_selection_surface_filled)
            {
                CutToSelectSurface(global_selection_rect);
            }
        }

        private void OnSelectionRectMoved(Rect obj)
        {
            _select_translate_x = obj.X;
            _select_translate_y = obj.Y;
        }

        private void OnNavigatorSourceRectChanged(Rect rect)
        {
            if (!SelectionEmpty() && _selection_surface_filled)
            {
                PasteFromSelectSurface();

                if (CurrentTool is SelectTool tool)
                {
                    tool.OnDeactivate();
                }
            }

            _sprite_source_rect = rect;
        }

        private void PickColorAt(int x, int y)
        {
            var pick_x = x;
            var pick_y = y;

            TypedMessager<byte>.Emit(MessageCodes.ColorPicked, CurrentSpritesheet.GetColorAt(pick_x, pick_y));
        }

        private Rect GetGlobalModifyRegion()
        {
            var source_rect = _sprite_source_rect;

            return SelectionEmpty() ? 
                source_rect : GetGlobalSelectionRect();
        }

        private Rect GetGlobalSelectionRect()
        {
            var (x, y) = LocalFramePointToSpriteSheetPoint(_select_src_x, _select_src_y);
            return new Rect(x, y, _select_width,
                _select_height);
        }

        private (int X, int Y) LocalFramePointToSpriteSheetPoint(int x, int y)
        {
            var source_rect = _sprite_source_rect;
            return (x + source_rect.X, y + source_rect.Y);
        }
             
        private bool SelectionEmpty()
        {
            return _select_width == 0 && _select_height == 0;
        }
    }
}
