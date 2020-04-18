using BLIT64;

namespace BLIT64_Editor
{
    public class ColorCell
    {
        public Rect Rect;
        public int Color;

        public ColorCell(int x, int y, int w, int h, int color)
        {
            Rect = new Rect(x, y, w, h);
            Color = color;
        }
    }

    public class ColorPicker : Component
    {
        public static int Width { get; } = 256;
        public static int Height { get; } = 0;

        public delegate void ColorPickerEvent(int color);

        public event ColorPickerEvent OnColorChange;

        public int CurrentColor { get; private set; } = 1;

        public static readonly int HorizontalCells = 16;
        public static readonly int PanelBorderSize = 3;
        public static readonly int CellSpacing = 2;

        private Palette _current_palette;
        private int _cell_size;
        private ColorCell[] _color_cells;
        private bool _mouse_down;


        public ColorPicker(Blitter blitter, Rect area, Palette palette) : base(blitter, area)
        {
            _current_palette = palette;

            BuildColorCells();
        }

        public void SetPalette(Palette palette)
        {
            _current_palette = palette;
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _mouse_down = true;
            PickAt(x, y);
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

            PickAt(x, y);
        }

        private void PickAt(int x, int y)
        {
            for (int i = 0; i < _color_cells.Length; ++i)
            {
                var cell = _color_cells[i];
                if (cell.Rect.Contains(x, y))
                {
                    CurrentColor = cell.Color;
                    OnColorChange?.Invoke(CurrentColor);
                    break;
                }
            }
        }

        public override void Draw()
        {
            var blitter = _blitter;

            blitter.Rect(_area.X, _area.Y, _area.W, _area.H, 35);

            blitter.Rect(
                x: _area.X, 
                y: _area.Y + _area.H, 
                w: _area.W, 
                h: PanelBorderSize, 
                col_index: 0);

            for (int i = 0; i < _color_cells.Length; ++i)
            {
                var cell = _color_cells[i];

                blitter.Rect(
                    _area.X + cell.Rect.X,
                    _area.Y + cell.Rect.Y,
                    cell.Rect.W,
                    cell.Rect.H,
                    cell.Color
                );
            }

            // Draw current color cell bigger

            var current_cell = _color_cells[CurrentColor];
            blitter.RectBorder(
                _area.X + current_cell.Rect.X - CellSpacing,
                _area.Y + current_cell.Rect.Y - CellSpacing,
                current_cell.Rect.W + 2*CellSpacing,
                current_cell.Rect.H + 2*CellSpacing,
                CellSpacing,
                35
            );

            blitter.Rect(
                _area.X + current_cell.Rect.X - CellSpacing,
                _area.Y + current_cell.Rect.Y - CellSpacing,
                current_cell.Rect.W + 2*CellSpacing,
                current_cell.Rect.H + 2*CellSpacing,
                current_cell.Color
            );

        }

        private void BuildColorCells()
        {
            _color_cells = new ColorCell[_current_palette.Count];

            var cell_size = ((_area.W - ((HorizontalCells + 1) * CellSpacing)) / (float)HorizontalCells);

            _cell_size = Calc.FastCeilToInt(cell_size);

            var line = 0;
            var col = 0;

            for (int i = 0; i < _current_palette.Count; ++i)
            {
                _color_cells[i] = new ColorCell(
                     ((col * _cell_size) + CellSpacing * col) + CellSpacing,
                    ((line * _cell_size) + CellSpacing * line) + CellSpacing,
                    _cell_size, 
                    _cell_size,
                    i
                );

                col += 1;

                if (col > HorizontalCells-1)
                {
                    line += 1;
                    col = 0;
                }
            }

            Area = new Rect(Area.X, Area.Y, Area.W + CellSpacing, line * _cell_size + (line+1)*CellSpacing);
        }
    }
}
