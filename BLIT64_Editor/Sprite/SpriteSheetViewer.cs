using BLIT64;

namespace BLIT64_Editor
{
    internal class SpriteSheetViewer : SpriteSheetDisplayComponent
    {
        public static SpriteSheetViewer Instance { get; private set; }

        public ref Rect SourceRect => ref _current_source_rect;

        public int CurrentSpriteId { get; private set; }

        private Rect _current_source_rect;
        private int _last_cursor_x;
        private int _last_cursor_y;
        private bool _mouse_down;

        private const int PANEL_BORDER_SIZE = 2;


        public SpriteSheetViewer(Blitter blitter, Rect area, Rect source_rect) : base(blitter, area)
        {
            Instance = this;
            _current_source_rect = source_rect;
        }

        public void SetCursorSize(int multiplier)
        {
            this._current_source_rect = new Rect(
                this._current_source_rect.X,
                this._current_source_rect.Y,
                SpriteEditor.TileSize * multiplier,
                SpriteEditor.TileSize * multiplier
            );

            ClampSourceRect();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            (_last_cursor_x, _last_cursor_y) = GetTransformedMousePos(x, y);

            _mouse_down = true;

            UpdateSourceRectFromMouseCursor(_last_cursor_x, _last_cursor_y);
            UpdateSpriteIdFromSourceRect();
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

        private void UpdateSpriteIdFromSourceRect()
        {
            CurrentSpriteId = (_current_source_rect.X + _current_source_rect.Y * (CurrentSpritesheet.Width/SpriteEditor.TileSize))/SpriteEditor.TileSize;
        }

        private void ClampSourceRect()
        {
            var x = _current_source_rect.X;
            var y = _current_source_rect.Y;

            if (x < 0)
            {
                x = 0;
            }
            else if (x + _current_source_rect.W > CurrentSpritesheet.Width)
            {
                x = CurrentSpritesheet.Width - _current_source_rect.W;
            }

            if (y < 0)
            {
                y = 0;
            }
            else if (y + _current_source_rect.H > CurrentSpritesheet.Height)
            {
                y = CurrentSpritesheet.Height - _current_source_rect.H;
            }

            _current_source_rect = new Rect(x, y, _current_source_rect.W, _current_source_rect.H);

        }

        protected override (int X, int Y) GetTransformedMousePos(int x, int y)
        {
            var (transformed_x, transformed_y) = base.GetTransformedMousePos(x, y);

            return (
                (int) Calc.Snap(transformed_x - _current_source_rect.W/2, SpriteEditor.TileSize),
                (int) Calc.Snap(transformed_y - _current_source_rect.H/2, SpriteEditor.TileSize)
            );
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _mouse_down = false;
        }

        public override void OnMouseMove(int x, int y)
        {
            if (!_mouse_down)
            {
                return;
            }

            var (cursor_x, cursor_y) = GetTransformedMousePos(x, y);

            var delta_x = cursor_x - _last_cursor_x;
            var delta_y = cursor_y - _last_cursor_y;

            if (delta_x != 0 || delta_y != 0)
            {
                UpdateSourceRectFromMouseCursor(cursor_x, cursor_y);
                UpdateSpriteIdFromSourceRect();
                _last_cursor_x = cursor_x;
                _last_cursor_y = cursor_y;
            }
        }

        public override void Draw()
        {
            var blitter = _blitter;

            blitter.Rect(_area.X, _area.Y, _area.W, _area.H, 2);

            blitter.Pixmap(CurrentSpritesheet, _area.X, _area.Y, ref Rect.Empty, _area.W, _area.H);

            blitter.RectBorder(_area.X, _area.Y, _area.W, _area.H, Palette.WhiteColor, PANEL_BORDER_SIZE);

            var scale_factor = _area.W / CurrentSpritesheet.Width;

            blitter.RectBorder(
                _area.X + _current_source_rect.X * scale_factor,
                _area.Y + _current_source_rect.Y * scale_factor,
                _current_source_rect.W * scale_factor,
                _current_source_rect.H * scale_factor,
                Palette.WhiteColor,
                1 * scale_factor
            );

            blitter.Rect(
                x: _area.X - PANEL_BORDER_SIZE, 
                y: _area.Y + _area.H + PANEL_BORDER_SIZE, 
                w: _area.W + 2 * PANEL_BORDER_SIZE, 
                h: PANEL_BORDER_SIZE, 
                color: Palette.BlackColor);
        }

    }
}
