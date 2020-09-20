using BLIT64;

namespace BLIT64_Editor
{
    public class SelectorOption
    {
        public int Value { get; }
        
        public Rect Rect { get; set; }

        public SelectorOption(int value)
        {
            Value = value;
        }
    }

    public class OptionsSelector : Component
    {
        public delegate void SelectorEvent(int value);

        public event SelectorEvent OnChange;

        public Orientation Orientation { get; }

        private readonly SelectorOption[] _options;

        private int _index;

        private int _cell_size;

        private bool _mouse_down;

        private const int StepMargin = 2;

        public OptionsSelector(Blitter blitter, Rect area, int[] options, Orientation orientation = Orientation.Horizontal) : base(blitter, area)
        {
            _options = new SelectorOption[options.Length];

            for (int i = 0; i < options.Length; ++i)
            {
                _options[i] = new SelectorOption(options[i]);
            }

            Orientation = orientation;

            UpdateGeometry();
        }

        public void SetValue(int value)
        {
            for (int i = 0; i < _options.Length; ++i)
            {
                if (_options[i].Value == value)
                {
                    _index = i;
                    return;
                }
            }
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

                    _cell_size = (int)((_area.W - ((steps + 1) * StepMargin)) / (float)steps);

                    for (int i = 0; i < steps; ++i)
                    {
                        _options[i].Rect = new Rect(
                            (i * _cell_size) + (i * StepMargin) + StepMargin,
                            0,
                            _cell_size, 
                            _cell_size
                        );
                    }

                    break;
                case Orientation.Vertical:

                    _cell_size = (int)((_area.H - ((steps + 1) * StepMargin)) / (float)steps);
            
                    for (int i = 0; i < steps; ++i)
                    {
                        _options[i].Rect = new Rect(
                            0,
                            (i * _cell_size) + (i * StepMargin) + StepMargin,
                            _cell_size, 
                            _cell_size
                        );
                    }

                    break;
            }

            
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            var steps = _options.Length;
            var blitter = _blitter;

            // Draw Background Steps

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

            // Draw Background Bar

            switch (Orientation)
            {
                case Orientation.Horizontal:
                    blitter.Rect(_area.X + 5, _area.Y + _area.H/2 - 2, _area.W - 10, 2, Palette.WhiteColor);
                    blitter.RectBorder(_area.X + 5, _area.Y + _area.H/2 - 2, _area.W - 10, 2, Palette.BlackColor, 2);
                    break;
                case Orientation.Vertical:
                    blitter.Rect(_area.X + _area.W/2 - 2  , _area.Y + 5, 2, _area.H - 10, Palette.WhiteColor);
                    blitter.RectBorder(_area.X + _area.W/2 - 2  , _area.Y + 5, 2, _area.H - 10, Palette.BlackColor, 2);
                    break;
            }

            var current_option_rect = _options[_index].Rect.Deflate(3);
            
            // Draw Slider

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
                        OnChange?.Invoke(_options[_index].Value);
                        break;
                    }
                    break;
                case Orientation.Vertical:
                    for (int i = 0; i < _options.Length; ++i)
                    {
                        var option = _options[i];
                        if (!option.Rect.Contains(0, y)) continue;
                        _index = i;
                        OnChange?.Invoke(_options[_index].Value);
                        break;
                    }
                    break;
            }
        }
    }
}
