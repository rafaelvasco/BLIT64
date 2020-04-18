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

    public class Selector : Component
    {
        public delegate void SelectorEvent(int value);

        public event SelectorEvent OnChange;

        public static int Width { get; } = 70;

        private readonly SelectorOption[] _options;

        private int _index;

        private int _cell_size;

        private bool _mouse_down = false;

        private const int STEP_MARGIN = 2;

        public Selector(Blitter blitter, Rect area, int[] options) : base(blitter, area)
        {
            _options = new SelectorOption[options.Length];

            for (int i = 0; i < options.Length; ++i)
            {
                _options[i] = new SelectorOption(options[i]);
            }

            UpdateGeometry();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_down = true;
            UpdateIndexFromCursor(x);
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

            UpdateIndexFromCursor(x);
        }

        public override void OnMouseLeave()
        {
            _mouse_down = false;
        }

        private void UpdateGeometry()
        {
            var steps = _options.Length;
            _cell_size = (int)((_area.W - ((steps + 1) * STEP_MARGIN)) / (float)steps);
            _area = new Rect(_area.X, _area.Y, _area.W, _cell_size);
            
            var cell_size = _cell_size;

            for (int i = 0; i < steps; ++i)
            {
                _options[i].Rect = new Rect(
                    (i * cell_size) + (i * STEP_MARGIN) + STEP_MARGIN,
                    0,
                    cell_size, 
                    cell_size
                );
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
                    1
                );
            }

            blitter.Rect(_area.X + 5, _area.Y + _area.H/2, _area.W - 10, 2, 35);

            var current_option_rect = _options[_index].Rect.Deflate(2);
            
            blitter.Rect(
                _area.X + current_option_rect.X,
                _area.Y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H,
                35
            );

            blitter.RectBorder(
                _area.X + current_option_rect.X,
                _area.Y + current_option_rect.Y,
                current_option_rect.W,
                current_option_rect.H,
                2
            );
        }

        private void UpdateIndexFromCursor(int x)
        {
            for (int i = 0; i < _options.Length; ++i)
            {
                var option = _options[i];
                if (!option.Rect.Contains(x, 0)) continue;
                _index = i;
                OnChange?.Invoke(_options[_index].Index);
                break;
            }
        }
    }
}
