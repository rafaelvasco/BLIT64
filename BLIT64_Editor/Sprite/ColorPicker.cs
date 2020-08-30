using BLIT64;

namespace BLIT64_Editor
{
    public class ColorCell
    {
        public Rect Rect;
        public byte Color;

        public ColorCell(int x, int y, int w, int h, byte color)
        {
            Rect = new Rect(x, y, w, h);
            Color = color;
        }
    }

    public class ColorPicker : Component
    {
        public delegate void ColorPickerEvent(byte color);

        public event ColorPickerEvent OnChange = delegate(byte color) {};

        public byte CurrentColor { get; private set; } = 1;

        public static readonly int HorizontalCells = 16;
        public static readonly int PanelBorderSize = 2;
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

        public void SetColor(byte color)
        {
            CurrentColor = color;
            OnChange.Invoke(color);
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

            x = Calc.Clamp(x, PanelBorderSize, _area.W-2*PanelBorderSize);
            y = Calc.Clamp(y, PanelBorderSize, _area.H-2*PanelBorderSize);

            PickAt(x, y);
        }

        private void PickAt(int x, int y)
        {
            for (int i = 0; i < _color_cells.Length; ++i)
            {
                var cell = _color_cells[i];
                if (cell.Rect.Contains(x, y))
                {
                    SetColor(cell.Color);
                    break;
                }
            }
        }

        public override void Draw()
        {
            var blitter = _blitter;

            blitter.Rect(_area.X, _area.Y, _area.W, _area.H, Palette.WhiteColor);

            blitter.Rect(
                x: _area.X, 
                y: _area.Y + _area.H, 
                w: _area.W, 
                h: PanelBorderSize, 
                color: Palette.BlackColor);

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
            var current_cell_rect = new Rect(
                _area.X + current_cell.Rect.X - PanelBorderSize,
                _area.Y + current_cell.Rect.Y - PanelBorderSize,
                current_cell.Rect.W + 2*PanelBorderSize,
                current_cell.Rect.H + 2*PanelBorderSize
            );
            

            blitter.Rect(
                current_cell_rect.X, 
                current_cell_rect.Y,
                current_cell_rect.W,
                current_cell_rect.H,
                current_cell.Color
            );

            blitter.RectBorder(
                current_cell_rect.X, 
                current_cell_rect.Y,
                current_cell_rect.W,
                current_cell_rect.H,
                Palette.WhiteColor,
                PanelBorderSize
            );

            blitter.Rect(
                x: current_cell_rect.X - PanelBorderSize, 
                y: current_cell_rect.Y + current_cell_rect.H + PanelBorderSize, 
                w: current_cell_rect.W + PanelBorderSize*2, 
                h: PanelBorderSize, 
                color: Palette.BlackColor);


            var transparent_col_cell = _color_cells[0];

            blitter.RectBorder
            (
                _area.X + transparent_col_cell.Rect.X + transparent_col_cell.Rect.W/2 - 2, 
                _area.Y + transparent_col_cell.Rect.Y + transparent_col_cell.Rect.H/2 - 2, 
                4, 
                4, Palette.WhiteColor, 1
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
                    (byte)i
                );

                col += 1;

                if (col > HorizontalCells-1)
                {
                    line += 1;
                    col = 0;
                }
            }

            Area = new Rect(Area.X, Area.Y, Area.W + PanelBorderSize, line * _cell_size + (line+1)*PanelBorderSize);
        }
    }
}
