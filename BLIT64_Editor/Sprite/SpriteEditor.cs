using System.Collections.Generic;
using BLIT64;
using BLIT64_Common;

namespace BLIT64_Editor
{
    public class SpriteEditor : Scene
    {
        public static int TileSize { get; private set; }
        
        private Rect _main_panel;
        private SpriteSheet _sprite_sheet;
        private Palette _current_palette;
        private SpriteEditorLayout _layout;
        private Component _hovered_component;
        private Component _active_component;

        private readonly List<Component> _components = new List<Component>();

        private SpriteSheetEditor _sprite_sheet_editor;
        private SpriteSheetViewer _sprite_sheet_viewer;
        private ColorPicker _color_picker;
        private Slider _source_rect_size_mult_slider;
        private ToolSelector _tool_box;

        public override void Load()
        {
            TileSize = Game.TileSize;

            var game_w = Game.Size.Width;
            var game_h = Game.Size.Height;

            var layout_file = BonFileReader.Parse("layout.bon");

            _layout = new SpriteEditorLayout()
            {
                ColorPickerWidth = layout_file.Main.ValueProps["colorpicker_width"].GetIntValue(),
                EditorCursorBorder = layout_file.Main.ValueProps["editor_cursor_border"].GetIntValue(),
                EditorPanelBorder = layout_file.Main.ValueProps["editor_panel_border"].GetIntValue(),
                EditorSizeMultiplier = layout_file.Main.ValueProps["editor_size_multiplier"].GetIntValue(),
                MainPanelMargin = layout_file.Main.ValueProps["main_panel_margin"].GetIntValue(),
                SelectorHeight = layout_file.Main.ValueProps["selector_height"].GetIntValue(),
                SelectorWidth = layout_file.Main.ValueProps["selector_width"].GetIntValue(),
                ToolBoxWidth = layout_file.Main.ValueProps["toolbox_width"].GetIntValue(),
                ToolBoxHeight = layout_file.Main.ValueProps["toolbox_height"].GetIntValue(),
                ToolBoxIconsScale = layout_file.Main.ValueProps["toolbox_icons_scale"].GetFloatValue(),
                ToolBoxIconsSpacing = layout_file.Main.ValueProps["toolbox_icons_spacing"].GetIntValue(),
                ToolBoxMargin = layout_file.Main.ValueProps["toolbox_margin"].GetIntValue(),
                ToolBoxPointerMargin = layout_file.Main.ValueProps["toolbox_pointer_margin"].GetIntValue(),
                ToolBoxIconsShadowOffset = layout_file.Main.ValueProps["toolbox_icon_shadow_offset"].GetIntValue(),
                ViewerSizeMultiplier = layout_file.Main.ValueProps["viewer_size_multiplier"].GetIntValue()
                
            };

            var pixmap_surface_size = Game.TileSize * 16;

            _sprite_sheet = Assets.CreateSpriteSheet(pixmap_surface_size, pixmap_surface_size);

            _current_palette = Palettes.Journey;

            _main_panel = new Rect(
                game_w / 2 - (game_w - _layout.MainPanelMargin) / 2,
                game_h / 2 - (game_h - _layout.MainPanelMargin) / 2,
                game_w - _layout.MainPanelMargin,
                game_h - _layout.MainPanelMargin);


            var sprite_editor_size = Game.TileSize * _layout.EditorSizeMultiplier;

            _sprite_sheet_editor = new SpriteSheetEditor(
                _layout,
                Blitter,
                new Rect(
                    _main_panel.X + _main_panel.W / 4 - sprite_editor_size / 2,
                    _main_panel.Y + _layout.MainPanelMargin,
                    sprite_editor_size,
                    sprite_editor_size
            ));

            _tool_box = new ToolSelector(
                _layout, 
                Blitter, 
                new Rect(
                _sprite_sheet_editor.Area.X + _sprite_sheet_editor.Area.W/2 - _layout.ToolBoxWidth/2,
                _sprite_sheet_editor.Area.Y + _sprite_sheet_editor.Area.H + _layout.ToolBoxMargin,
                _layout.ToolBoxWidth,
                _layout.ToolBoxHeight
                
            ));

            var sprite_viewer_size = Game.TileSize * _layout.ViewerSizeMultiplier;

            _sprite_sheet_viewer = new SpriteSheetViewer(
                Blitter,
                new Rect(
                _main_panel.X + _main_panel.W / 2 + _main_panel.W / 4 - sprite_viewer_size / 2,
                _main_panel.Y + _main_panel.H / 2 - sprite_viewer_size / 2,
                sprite_viewer_size,
                sprite_viewer_size
            ), new Rect(0, 0, Game.TileSize, Game.TileSize));

            _color_picker = new ColorPicker(Blitter, new Rect(
                
                _sprite_sheet_editor.Area.X + _sprite_sheet_editor.Area.W / 2 - _layout.ColorPickerWidth/2, 
                _sprite_sheet_editor.Area.Bottom + 50,
                _layout.ColorPickerWidth,
                0

            ), _current_palette);

            _source_rect_size_mult_slider = new Slider(Blitter, new Rect(
                _sprite_sheet_viewer.Area.X + _sprite_sheet_viewer.Area.W/2 - _layout.SelectorWidth/2,
                _sprite_sheet_viewer.Area.Y - 25,
                _layout.SelectorWidth,
                _layout.SelectorHeight
                
            ), new []
            {
                1, 2, 4, 8
            });

            

            _sprite_sheet_editor.SetSpriteSheet(_sprite_sheet);
            _sprite_sheet_viewer.SetSpriteSheet(_sprite_sheet);
            _color_picker.SetPalette(_current_palette);
            _sprite_sheet_editor.SetPaintColor(_color_picker.CurrentColor);

            _components.Add(_sprite_sheet_editor);
            _components.Add(_sprite_sheet_viewer);
            _components.Add(_color_picker);
            _components.Add(_tool_box);
            _components.Add(_source_rect_size_mult_slider);

            Input.AddMouseDownListener(OnMouseDown);
            Input.AddMouseUpListener(OnMouseUp);
            Input.AddMouseMoveListener(OnMouseMove);
            Input.AddKeyDownListener(OnKeyDown);
            Input.AddKeyUpListener(OnKeyUp);

            _color_picker.OnChange += OnPickerChange;
            _source_rect_size_mult_slider.OnChange += OnSourceRectSliderChange;
            _sprite_sheet_editor.OnColorPick += OnColorPick;
            _tool_box.On((int) Actions.ChangeTool, OnToolBoxChangeTool);
            _tool_box.On((int) Actions.ModifySprite, OnToolBoxModifierTriggered);

        }

        private void OnToolBoxModifierTriggered(object obj)
        {
            var modifier = (Modifiers) obj;

            switch (modifier)
            {
                case Modifiers.FlipH:
                    _sprite_sheet_editor.FlipH();
                    break;
                case Modifiers.FlipV:
                    _sprite_sheet_editor.FlipV();
                    break;
                case Modifiers.Rotate:
                    _sprite_sheet_editor.Rotate();
                    break;
                case Modifiers.Clear:
                    _sprite_sheet_editor.ClearFrame();
                    break;
            }
        }

        private void OnToolBoxChangeTool(object obj)
        {
            var tool = (Tools)obj;
            _sprite_sheet_editor.SetCurrentTool(tool);
        }

        private void OnColorPick(byte color)
        {
            _color_picker.SetColor(color);
        }

        private void OnSourceRectSliderChange(int value)
        {
            _sprite_sheet_viewer.SetCursorSize(value);
        }

        private void OnPickerChange(byte color)
        {
            _sprite_sheet_editor.SetPaintColor(color);
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

            if (_hovered_component != null)
            {
                _hovered_component.OnMouseDown(button, mouse_x - _hovered_component.Area.X, mouse_y - _hovered_component.Area.Y);
                _active_component = _hovered_component;
            }
        }

        private void OnMouseUp(MouseButton button)
        {
            var (mouse_x, mouse_y) = Input.MousePos;

            if (_active_component == null)
            {
                _hovered_component?.OnMouseUp(button, mouse_x - _hovered_component.Area.X, mouse_y - _hovered_component.Area.Y);
            }
            else
            {
                _active_component.OnMouseUp(button, mouse_x - _hovered_component.Area.X, mouse_y - _hovered_component.Area.Y);
                _active_component = null;
            }
            
        }

        private void OnMouseMove()
        {
            var (mouse_x, mouse_y) = Input.MousePos;

            if (_active_component == null)
            {
                for (int i = _components.Count-1; i >= 0; --i)
                {
                    var component = _components[i];
                    if (component.MouseOver(mouse_x, mouse_y))
                    {
                        if (_hovered_component != component)
                        {
                            _hovered_component = component;
                            _hovered_component.OnMouseEnter();
                            _hovered_component.Hovered = true;
                        }
                        component.OnMouseMove(mouse_x - component.Area.X, mouse_y - component.Area.Y);
                        return;
                    }
                    else
                    {
                        if (_hovered_component == component)
                        {
                            _hovered_component.OnMouseLeave();
                            _hovered_component.Hovered = false;
                            _hovered_component = null;
                        }
                    }
                }
            }
            else
            {
                _active_component.OnMouseMove(mouse_x - _active_component.Area.X, mouse_y - _active_component.Area.Y);
                if (_active_component.MouseOver(mouse_x, mouse_y) && !_active_component.Hovered)
                {
                    _active_component.Hovered = true;
                    _active_component.OnMouseEnter();
                }
                else if(!_active_component.MouseOver(mouse_x, mouse_y) && _active_component.Hovered)
                {
                    _active_component.Hovered = false;
                    _active_component.OnMouseLeave();
                }
            }
            
        }

        public override void Update()
        {
        }

        public override void Draw(Blitter blitter)
        {
            blitter.Clear();

            blitter.Rect(
                _main_panel.X,
                _main_panel.Y,
                _main_panel.W,
                _main_panel.H, 33);

            foreach (var component in _components)
            {
                component.Draw();
            }
        }
    }
}
