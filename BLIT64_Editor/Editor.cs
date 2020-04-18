using System.Collections.Generic;
using BLIT64;

namespace BLIT64_Editor
{
    public class Editor : Scene
    {
        public static readonly int TileSize = 8;
        public static readonly int PixmapSurfaceSize = TileSize * 16;
        private const int MAIN_PANEL_MARGIN = 40;

        private Rect _main_panel;
        private Pixmap _sprite_sheet;
        private Palette _current_palette;

        private Component _hovered_component;

        private readonly List<Component> _components = new List<Component>();

        private PixmapEditor _pixmap_editor;
        private PixmapViewer _pixmap_viewer;
        private ColorPicker _color_picker;

        private Selector _source_rect_size_mult_selector;

        public override void Load()
        {
            var game_w = Game.Size.Width;
            var game_h = Game.Size.Height;

            _sprite_sheet = Assets.CreatePixmap(PixmapSurfaceSize, PixmapSurfaceSize);

            _current_palette = Palettes.Journey;

            _main_panel = new Rect(
                game_w / 2 - (game_w - MAIN_PANEL_MARGIN) / 2,
                game_h / 2 - (game_h - MAIN_PANEL_MARGIN) / 2,
                game_w - MAIN_PANEL_MARGIN,
                game_h - MAIN_PANEL_MARGIN);


            _pixmap_editor = new PixmapEditor(
                Blitter,
                new Rect(
                    _main_panel.X + _main_panel.W / 4 - PixmapEditor.Size / 2,
                    _main_panel.Y + MAIN_PANEL_MARGIN,
                    PixmapEditor.Size,
                    PixmapEditor.Size
            ));

            _pixmap_viewer = new PixmapViewer(
                Blitter,
                new Rect(
                _main_panel.X + _main_panel.W / 2 + _main_panel.W / 4 - PixmapViewer.Size / 2,
                _main_panel.Y + _main_panel.H / 2 - PixmapViewer.Size / 2,
                PixmapViewer.Size,
                PixmapViewer.Size

            ), new Rect(
                0, 0, TileSize, TileSize
            ));

            _color_picker = new ColorPicker(Blitter, new Rect(
                
                _pixmap_editor.Area.X + _pixmap_editor.Area.W / 2 - ColorPicker.Width/2, 
                _pixmap_editor.Area.Bottom + 50,
                ColorPicker.Width,
                ColorPicker.Height

            ), _current_palette);

            _source_rect_size_mult_selector = new Selector(Blitter, new Rect(
                _pixmap_viewer.Area.X + _pixmap_viewer.Area.W/2 - Selector.Width/2,
                _pixmap_viewer.Area.Y - 25,
                Selector.Width,
                0
                
            ), new []
            {
                1, 2, 4, 8
            });

            _source_rect_size_mult_selector.OnChange += OnSourceRectSelectorChange;

            _pixmap_editor.SetPixmap(_sprite_sheet);
            _pixmap_viewer.SetPixmap(_sprite_sheet);
            _color_picker.SetPalette(_current_palette);

            _color_picker.OnColorChange += OnColorPickerColorChange;

            _components.Add(_pixmap_editor);
            _components.Add(_pixmap_viewer);
            _components.Add(_color_picker);
            _components.Add(_source_rect_size_mult_selector);

            Input.AddMouseDownListener(OnMouseDown);
            Input.AddMouseUpListener(OnMouseUp);
            Input.AddMouseMoveListener(OnMouseMove);
            Input.AddKeyDownListener(OnKeyDown);
            Input.AddKeyUpListener(OnKeyUp);

        }

        private void OnSourceRectSelectorChange(int value)
        {
            _pixmap_viewer.SetCursorSize(value);
        }

        private void OnColorPickerColorChange(int color)
        {
            _pixmap_editor.SetPaintColor(color);
        }

        private void OnKeyUp(Key key)
        {
            foreach (var component in _components)
            {
                component.OnKeyUp(key);
            }
        }

        private void OnKeyDown(Key key)
        {
            foreach (var component in _components)
            {
                component.OnKeyDown(key);
            }
        }

        private void OnMouseDown(MouseButton button)
        {
            var (mouse_x, mouse_y) = Input.MousePos;

            _hovered_component?.OnMouseDown(button, mouse_x - _hovered_component.Area.X, mouse_y - _hovered_component.Area.Y);
        }

        private void OnMouseUp(MouseButton button)
        {
            var (mouse_x, mouse_y) = Input.MousePos;

            _hovered_component?.OnMouseUp(button, mouse_x - _hovered_component.Area.X, mouse_y - _hovered_component.Area.Y);
        }

        private void OnMouseMove()
        {
            var (mouse_x, mouse_y) = Input.MousePos;

            foreach (var component in _components)
            {
                if (component.MouseOver(mouse_x, mouse_y))
                {
                    if (_hovered_component != component)
                    {
                        _hovered_component = component;
                        _hovered_component.OnMouseEnter();
                    }
                    component.OnMouseMove(mouse_x - component.Area.X, mouse_y - component.Area.Y);
                    return;
                }
                else
                {
                    if (_hovered_component == component)
                    {
                        _hovered_component.OnMouseLeave();
                        _hovered_component = null;
                    }
                }
            }
        }

        public override void Update()
        {
        }

        public override void Draw(Blitter blitter)
        {
            blitter.Clear(1);

            blitter.Rect(
                _main_panel.X,
                _main_panel.Y,
                _main_panel.W,
                _main_panel.H, 31);

            foreach (var component in _components)
            {
                component.Draw();
            }

            var (mouse_x, mouse_y) = Input.MousePos;

            blitter.Text(50, 25, $"Mouse Pos: {mouse_x},{mouse_y}; Hovered Color: {_color_picker.CurrentColor}");
        }
    }
}
