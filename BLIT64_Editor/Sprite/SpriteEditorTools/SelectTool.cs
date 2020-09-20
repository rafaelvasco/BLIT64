using System;
using BLIT64;
using BLIT64_Editor.Common;

namespace BLIT64_Editor
{
    public class SelectTool : Tool
    {
        private int _start_x;
        private int _start_y;
        private int _last_drag_x;
        private int _last_drag_y;
        private int _x;
        private int _y;
        private int _x2;
        private int _y2;
        private Rect _rect;
        private bool _selecting;
        private bool _dragging;
        private bool _started_dragging;

        public override int BrushSize
        {
            get => _brush_size;
            set { }
        }

        public SelectTool()
        {
            _brush_size = 1;
        }


        private void TranslateSelectRect(int x, int y, Rect bounds)
        {
            _x += x;
            _y += y;

            _x = Math.Max(0, _x);
            _y = Math.Max(0, _y);

            if (_x + _rect.W > bounds.W)
            {
                _x = bounds.W - _rect.W;
            }

            if (_y + _rect.H > bounds.H)
            {
                _y = bounds.H - _rect.H;
            }

            _x2 = _x + _rect.W;
            _y2 = _y + _rect.H;

            EmitTranslated();
        }

        public override void OnDeactivate()
        {
            ClearSelection();
            _selecting = false;
            _dragging = false;
        }

        private void ClearSelection()
        {
            _start_x = 0;
            _start_y = 0;
            _x = 0;
            _y = 0;
            _x2 = 0;
            _y2 = 0;
            _selecting = false;
            _dragging = false;
            _started_dragging = false;

            EmitSizeChanged();
        }

        public override bool UseVariableBrushSize => false;

        public override void OnMouseDown(ToolActionParams @params)
        {
            if (@params.MouseButton == MouseButton.Left)
            {
                if (!_rect.IsEmpty && _rect.Contains(@params.CursorX, @params.CursorY))
                {
                    _dragging = true;
                    _selecting = false;
                    _last_drag_x = @params.CursorX;
                    _last_drag_y = @params.CursorY;
                }
                else
                {
                    if (_x2 - _x > 0 || _y2 - _x > 0)
                    {
                        ClearSelection();    
                    }
                    
                    _selecting = true;
                    _dragging = false;
                    _started_dragging = false;
                    _start_x = @params.CursorX;
                    _start_y = @params.CursorY;
                    _x = _start_x;
                    _y = _start_y;
                    _x2 = _start_x + _brush_size;
                    _y2 = _start_y + _brush_size;

                    EmitSizeChanged();
                }
            }
            else
            {
                ClearSelection();
            }

        }

        public override void OnMouseUp(ToolActionParams @params)
        {
            _selecting = false;
            _dragging = false;
        }

        public override void OnMouseMove(ToolActionParams @params)
        {
            if (!_selecting && !_dragging)
            {
                return;
            }

            if (_selecting)
            {
                _x = Math.Max(0, Math.Min(_start_x, @params.CursorX));
                _y = Math.Max(0, Math.Min(_start_y, @params.CursorY));

                _x2 = Math.Min(@params.SourceRect.W, Math.Max(_start_x, @params.CursorX) + _brush_size);
                _y2 = Math.Min(@params.SourceRect.H, Math.Max(_start_y, @params.CursorY) + _brush_size);

                EmitSizeChanged();
            }
            else if (_dragging)
            {
                var delta_x = @params.CursorX - _last_drag_x;
                var delta_y = @params.CursorY - _last_drag_y;

                if (delta_x != 0 || delta_y != 0)
                {
                    if (!_started_dragging)
                    {
                        _started_dragging = true;
                        EmitStartedMoving();
                    }
                }

                TranslateSelectRect(delta_x, delta_y, @params.SourceRect);

                _last_drag_x = @params.CursorX;
                _last_drag_y = @params.CursorY;
            }

        }

        public override void OnKeyDown(Key key, ToolActionParams @params)
        {
            if (!_started_dragging)
            {
                _started_dragging = true;
                EmitStartedMoving();
            }

            switch (key)
            {
                case Key.A:
                case Key.Left:
                    TranslateSelectRect(-1, 0, @params.SourceRect);
                    break;
                case Key.D:
                case Key.Right:
                    TranslateSelectRect(1, 0, @params.SourceRect);
                    break;
                case Key.W:
                case Key.Up:
                    TranslateSelectRect(0, -1, @params.SourceRect);
                    break;
                case Key.S:
                case Key.Down:
                    TranslateSelectRect(0, 1, @params.SourceRect);
                    break;
            }

           
        }

        private void EmitSizeChanged()
        {
            _rect = new Rect(_x, _y, _x2 - _x, _y2 - _y);
            TypedMessager<Rect>.Emit(MessageCodes.SelectionRectResize, _rect);
        }

        private void EmitTranslated()
        {
            _rect = new Rect(_x, _y, _x2 - _x, _y2 - _y);
            TypedMessager<Rect>.Emit(MessageCodes.SelectionRectMoved, _rect);
        }

        private void EmitStartedMoving()
        {
            Messager.Emit(MessageCodes.SelectionStartedMoving);
        }


        public override void Update(ToolActionParams @params)
        {
        }
    }
}
