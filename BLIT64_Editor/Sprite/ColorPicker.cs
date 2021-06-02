using BLIT64;
using BLIT64.Toolkit.Gui;
using BLIT64_Editor.Common;

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


    public sealed class ColorPicker : Container
    {
        public byte CurrentColor { get; private set; } = 1;

        public static readonly int HorizontalCells = 16;
        public static readonly int PanelBorderSize = 2;
        public static readonly int CellSpacing = 2;
        

        private int _current_palette;
        private int _cell_size;
        private ColorCell[] _color_cells;
        private Button _choose_palette_button;
        private bool _mouse_down;


        public ColorPicker(string id) : base(id, AppLayout.Data.ColorPickerWidth, 0)
        {
            _current_palette = 1;

            BuildColorCells();

            _choose_palette_button = new Button("choose_palette_button", "P", 10, Height-2);

            _choose_palette_button.OnClick += OnPaletteChooseClick;

            Add(_choose_palette_button);

            _choose_palette_button.Align(Alignment.Right);

            _choose_palette_button.Y = 2;
            _choose_palette_button.X += _choose_palette_button.Width + 2;
        }

        private void OnPaletteChooseClick()
        {
            
        }

        public void SetColor(byte color)
        {
            CurrentColor = color;
            TypedMessager<byte>.Emit(MessageCodes.ColorSelected, color);
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

            x = Calc.Clamp(x, PanelBorderSize, Width-2*PanelBorderSize);
            y = Calc.Clamp(y, PanelBorderSize, Height-2*PanelBorderSize);

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

        public override void Update()
        {
        }

        public override void Draw(Canvas blitter, IGuiDrawer drawer)
        {
            blitter.BeginDraw(_current_palette);
            blitter.Clip(DrawX, DrawY, Width, Height);

            blitter.SetColor(Palette.WhiteColor);
            blitter.RectFill(DrawX, DrawY, Width, Height);

            blitter.SetColor(Palette.BlackColor);
            blitter.RectFill(
                x: DrawX, 
                y: DrawY + Height, 
                w: Width, 
                h: PanelBorderSize);


            for (int i = 0; i < _color_cells.Length; ++i)
            {
                var cell = _color_cells[i];

                blitter.SetColor(cell.Color);
                blitter.RectFill(
                    DrawX + cell.Rect.X,
                    DrawY + cell.Rect.Y,
                    cell.Rect.W,
                    cell.Rect.H
                );

            }

            // Draw current color cell bigger

            var current_cell = _color_cells[CurrentColor];
            var current_cell_rect = new Rect(
                DrawX + current_cell.Rect.X - PanelBorderSize,
                DrawY + current_cell.Rect.Y - PanelBorderSize,
                current_cell.Rect.W + 2*PanelBorderSize,
                current_cell.Rect.H + 2*PanelBorderSize
            );
            
            blitter.SetColor(current_cell.Color);
            blitter.RectFill(
                current_cell_rect.X, 
                current_cell_rect.Y,
                current_cell_rect.W,
                current_cell_rect.H
            );

            blitter.SetColor(Palette.WhiteColor);
            blitter.Rect(
                current_cell_rect.X, 
                current_cell_rect.Y,
                current_cell_rect.W,
                current_cell_rect.H,
                PanelBorderSize
            );

            blitter.SetColor(Palette.BlackColor);
            blitter.RectFill(
                x: current_cell_rect.X - PanelBorderSize, 
                y: current_cell_rect.Y + current_cell_rect.H + PanelBorderSize, 
                w: current_cell_rect.W + PanelBorderSize*2, 
                h: PanelBorderSize);


            var transparent_col_cell = _color_cells[0];

            blitter.SetColor(Palette.WhiteColor);
            blitter.Rect
            (
                DrawX + transparent_col_cell.Rect.X + transparent_col_cell.Rect.W/2 - 2, 
                DrawY + transparent_col_cell.Rect.Y + transparent_col_cell.Rect.H/2 - 2, 
                w: 4, 
                h: 4, 
                line_size: 1
            );

            blitter.Clip();
            blitter.EndDraw();

            DrawChildren(blitter, drawer);
        }

        private void BuildColorCells()
        {
            int color_count = Palette.ColorCount;

            _color_cells = new ColorCell[color_count];

            var cell_size = ((Width - ((HorizontalCells + 1) * CellSpacing)) / (float)HorizontalCells);

            _cell_size = Calc.FastCeilToInt(cell_size);

            var line = 0;
            var col = 0;

            for (int i = 0; i < color_count; ++i)
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

            Width += PanelBorderSize;
            Height = line * _cell_size + (line + 1) * PanelBorderSize;
        }
    }
}
