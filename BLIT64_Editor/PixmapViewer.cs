using System.IO.Compression;
using BLIT64;

namespace BLIT64_Editor
{
    internal class PixmapViewer : PixmapDisplayComponent
    {
        public static readonly int Size = Editor.TileSize * 32;

        public static PixmapViewer Instance { get; private set; }

        public ref Rect SourceRect => ref _current_source_rect;

        private Rect _current_source_rect;
        private int _last_cursor_x;
        private int _last_cursor_y;
        private bool _mouse_down;

        private const int PANEL_BORDER_SIZE = 3;


        public PixmapViewer(Blitter blitter, Rect area, Rect source_rect) : base(blitter, area)
        {
            Instance = this;
            _current_source_rect = source_rect;
        }

        public void SetCursorSize(int multiplier)
        {
            this._current_source_rect = new Rect(
                this._current_source_rect.X,
                this._current_source_rect.Y,
                Editor.TileSize * multiplier,
                Editor.TileSize * multiplier
            );

            ClampSourceRect();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            (_last_cursor_x, _last_cursor_y) = GetLocalTransformedMousePos(x, y);

            _mouse_down = true;

            UpdateSourceRectFromMouseCursor(_last_cursor_x, _last_cursor_y);
        }

        private void UpdateSourceRectFromMouseCursor(int cursor_x, int cursor_y)
        {
            _current_source_rect = new Rect(
                cursor_x,
                cursor_y,
                _current_source_rect.W,
                _current_source_rect.H
            );

            ClampSourceRect();
        }

        private void ClampSourceRect()
        {
            var x = _current_source_rect.X;
            var y = _current_source_rect.Y;

            if (x < 0)
            {
                x = 0;
            }
            else if (x + _current_source_rect.W > _current_pixmap.Width)
            {
                x = _current_pixmap.Width - _current_source_rect.W;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y + _current_source_rect.H > _current_pixmap.Height)
            {
                y = _current_pixmap.Height - _current_source_rect.H;
            }

            _current_source_rect = new Rect(x, y, _current_source_rect.W, _current_source_rect.H);

        }

        protected override (int X, int Y) GetLocalTransformedMousePos(int x, int y)
        {
            var (transformed_x, transformed_y) = base.GetLocalTransformedMousePos(x, y);

            return (
                (int) Calc.Snap(transformed_x - _current_source_rect.W/2, Editor.TileSize),
                (int) Calc.Snap(transformed_y - _current_source_rect.H/2, Editor.TileSize)
            );
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _mouse_down = false;
        }

        public override void OnMouseLeave()
        {
            _mouse_down = false;
        }

        public override void OnMouseMove(int x, int y)
        {
            if (!_mouse_down)
            {
                return;
            }

            var (cursor_x, cursor_y) = GetLocalTransformedMousePos(x, y);

            var delta_x = cursor_x - _last_cursor_x;
            var delta_y = cursor_y - _last_cursor_y;

            if (delta_x != 0 || delta_y != 0)
            {
                UpdateSourceRectFromMouseCursor(cursor_x, cursor_y);
                _last_cursor_x = cursor_x;
                _last_cursor_y = cursor_y;
            }
        }

        public override void Draw()
        {
            var blitter = _blitter;

            blitter.Pixmap(_current_pixmap, _area.X, _area.Y, Rect.Empty, _area.W, _area.H);

            blitter.RectBorder(_area.X, _area.Y, _area.W, _area.H, PANEL_BORDER_SIZE, 35);

            var scale_factor = _area.W / _current_pixmap.Width;

            blitter.RectBorder(
                _area.X + _current_source_rect.X * scale_factor,
                _area.Y + _current_source_rect.Y * scale_factor,
                _current_source_rect.W * scale_factor,
                _current_source_rect.H * scale_factor,
                1 * scale_factor,
                35
            );

            blitter.Rect(
                x: _area.X - PANEL_BORDER_SIZE, 
                y: _area.Y + _area.H + PANEL_BORDER_SIZE, 
                w: _area.W + 2 * PANEL_BORDER_SIZE, 
                h: PANEL_BORDER_SIZE, 
                col_index: 0);
        }
    }
}
