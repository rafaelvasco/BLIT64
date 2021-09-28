using BLIT64;
using BLIT64.Toolkit.Gui;
using BLIT64_Editor.Common;

namespace BLIT64_Editor
{
    internal class SpriteSheetNavigator : SpriteSheetDisplayComponent
    {
        public static SpriteSheetNavigator Instance { get; private set; }

        public int CurrentSpriteId { get; private set; }

        private Rect _current_source_rect;
        private int _last_cursor_x;
        private int _last_cursor_y;
        private bool _mouse_down;
        private readonly AppLayout.LayoutData _layout = AppLayout.Data;

        public SpriteSheetNavigator(string id, int width, int height, Rect source_rect) : base(id, width, height)
        {
            Instance = this;
            _current_source_rect = source_rect;

            EmitRectChanged();

            TypedMessager<int>.On(MessageCodes.SpriteNavigatorCursorSizeChanged, SetCursorSize);
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

            EmitRectChanged();

            UpdateSpriteIdFromSourceRect();
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

            EmitRectChanged();
        }

        private void UpdateSpriteIdFromSourceRect()
        {
            CurrentSpriteId = (_current_source_rect.X + _current_source_rect.Y * (CurrentSpritesheet.Width/SpriteEditor.TileSize))/SpriteEditor.TileSize;
            TypedMessager<int>.Emit(MessageCodes.SpriteIdChanged, CurrentSpriteId);
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
                (int) Calc.Snap(transformed_x, SpriteEditor.TileSize),
                (int) Calc.Snap(transformed_y, SpriteEditor.TileSize)
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

        public override void Draw(Canvas canvas, IGuiDrawer drawer)
        {
            var panel_border_size = _layout.NavigatorPanelBorder;

            canvas.SetColor(35);

            canvas.Text(DrawX + _layout.NavigatorFrameLabelOffsetX, DrawY + _layout.NavigatorFrameLabelOffsetY, $"Frame: {_current_source_rect.W}x{_current_source_rect.H}");

            canvas.SetColor(AppColors.BackColor);
            canvas.RectFill(DrawX, DrawY, Width, Height);

            canvas.Pixmap(CurrentSpritesheet, DrawX, DrawY, Rect.Empty, Width, Height);

            canvas.SetColor(AppColors.BorderColor);
            canvas.Rect(DrawX, DrawY, Width, Height, panel_border_size);

            var scale_factor = Width / CurrentSpritesheet.Width;

            canvas.Rect(
                (DrawX + _current_source_rect.X * scale_factor),
                (DrawY + _current_source_rect.Y * scale_factor),
                (_current_source_rect.W * scale_factor),
                (_current_source_rect.H * scale_factor),
                1 * scale_factor
            );

            canvas.SetColor(AppColors.BackColor);
            canvas.RectFill(
                    x: DrawX - panel_border_size,
                    y: DrawY + Height + panel_border_size,
                    w: Width + panel_border_size * 2,
                    h: panel_border_size);

        }

        private void EmitRectChanged()
        {
            TypedMessager<Rect>.Emit(MessageCodes.SourceRectChanged, _current_source_rect);
        }

    }
}
