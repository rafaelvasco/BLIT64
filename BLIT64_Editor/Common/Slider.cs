using System;
using BLIT64;

namespace BLIT64_Editor
{
    public class SelectorOption
    {
        public int Index { get; }
        
        public Rect Rect { get; set; }

        public SelectorOption(int index)
        {
            Index = index;
        }
    }

    public class Slider : Component
    {
        public delegate void SelectorEvent(int value);

        public event SelectorEvent OnChange;

        public Orientation Orientation { get; }

        private readonly SelectorOption[] _options;

        private int _index;

        private int _cell_size;

        private bool _mouse_down;

        private const int STEP_MARGIN = 2;

        public Slider(Blitter blitter, Rect area, int[] options, Orientation orientation = Orientation.Horizontal) : base(blitter, area)
        {
            _options = new SelectorOption[options.Length];

            for (int i = 0; i < options.Length; ++i)
            {
                _options[i] = new SelectorOption(options[i]);
            }

            Orientation = orientation;

            UpdateGeometry();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_down = true;
            UpdateIndexFromCursor(x, y);
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

            UpdateIndexFromCursor(x, y);
        }

        private void UpdateGeometry()
        {
            var steps = _options.Length;

            switch (Orientation)
            {
                case Orientation.Horizontal:

                    _cell_size = (int)((_area.W - ((steps + 1) * STEP_MARGIN)) / (float)steps);
                    _area = new Rect(_area.X, _area.Y, _area.W, _cell_size);

                    for (int i = 0; i < steps; ++i)
                    {
                        _options[i].Rect = new Rect(
                            (i * _cell_size) + (i * STEP_MARGIN) + STEP_MARGIN,
                            0,
                            _cell_size, 
                            _cell_size
                        );
                    }

                    break;
                case Orientation.Vertical:

                    _area = new Rect(_area.X, _area.Y, _area.H, _area.W);
                    _cell_size = (int)((_area.H - ((steps + 1) * STEP_MARGIN)) / (float)steps);
                    _area = new Rect(_area.X, _area.Y, _cell_size, _area.H);
            
                    var cell_size = _cell_size;

                    for (int i = 0; i < steps; ++i)
                    {
                        _options[i].Rect = new Rect(
                            0,
                            (i * cell_size) + (i * STEP_MARGIN) + STEP_MARGIN,
                            cell_size, 
                            cell_size
                        );
                    }

                    break;
            }

            
        }

        public override void Draw()
        {
            var steps = _options.Length;
            var blitter = _blitter;

            for (int i = 0; i < steps; ++i)
            {
                var option_rect = _options[i].Rect;
                blitter.Rect(
                    _area.X + option_rect.X, 
                    _area.Y + option_rect.Y, 
                    option_rect.W, 
                    option_rect.H,
                    Palette.BlackColor
                );
            }

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    blitter.Rect(_area.X + 5, _area.Y + _area.H/2, _area.W - 10, 2, Palette.WhiteColor);
                    blitter.RectBorder(_area.X + 5, _area.Y + _area.H/2, _area.W - 10, 2, Palette.BlackColor, 2);
                    break;
                case Orientation.Vertical:
                    blitter.Rect(_area.X + _area.W/2, _area.Y + 5, 2, _area.H - 10, Palette.WhiteColor);
                    blitter.RectBorder(_area.X + _area.W/2, _area.Y + 5, 2, _area.H - 10, Palette.WhiteColor, 2);
                    break;
            }

            var current_option_rect = _options[_index].Rect.Deflate(3);
            
            blitter.Rect(
                _area.X + current_option_rect.X,
                _area.Y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H,
                Palette.WhiteColor
            );

            blitter.RectBorder(
                _area.X + current_option_rect.X,
                _area.Y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H,
                Palette.BlackColor
            );
        }

        private void UpdateIndexFromCursor(int x, int y)
        {
            switch (Orientation)
            {
                case Orientation.Horizontal:
                    for (int i = 0; i < _options.Length; ++i)
                    {
                        var option = _options[i];
                        if (!option.Rect.Contains(x, 0)) continue;
                        _index = i;
                        OnChange?.Invoke(_options[_index].Index);
                        break;
                    }
                    break;
                case Orientation.Vertical:
                    for (int i = 0; i < _options.Length; ++i)
                    {
                        var option = _options[i];
                        if (!option.Rect.Contains(0, y)) continue;
                        _index = i;
                        OnChange?.Invoke(_options[_index].Index);
                        break;
                    }
                    break;
            }
        }
    }
}
