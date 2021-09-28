using BLIT64;
using BLIT64.Toolkit.Gui;
using BLIT64_Editor.Common;

namespace BLIT64_Editor
{
    public class SpriteEditor : Container
    {
        public static int TileSize { get; private set; }

        private readonly Label _sprite_id_label;
        private readonly SpriteSheetEditor _sprite_sheet_editor;
        private readonly ColorPicker _color_picker;
        private readonly SelectorSlider _brush_size_slider;

        public SpriteEditor(string id, int width, int height) : base(id, width, height)
        {
            TileSize = Game.Instance.TileSize;

            var pixmap_surface_size = TileSize * AppLayout.Data.EditorPixmapSizeMultiplier;

            var sprite_sheet = Assets.CreateSpriteSheet(pixmap_surface_size, pixmap_surface_size);

            var horizontal_container = new Container("hor_container", width, height);

            var left_vertical_container = new Container("left_vert_container", 0, 0);

            var right_vertical_container = new Container("right_vert_container", 0, 0);

            horizontal_container.Add(left_vertical_container);
            horizontal_container.Add(right_vertical_container);

            _sprite_id_label = new Label("sprite_id_label", $"#000");

            var sprite_editor_size = TileSize * AppLayout.Data.EditorSizeMultiplier;

            _sprite_sheet_editor =
                new SpriteSheetEditor("sprite_sheet_editor", sprite_editor_size, sprite_editor_size);


            var tool_box = new ToolSelector("tool_selector");
            
            var spritesheet_navigator_size = TileSize * AppLayout.Data.NavigatorSizeMultiplier;

            var sprite_sheet_navigator = new SpriteSheetNavigator("sprite_sheet_nav", spritesheet_navigator_size,
                spritesheet_navigator_size, new Rect(0, 0, TileSize, TileSize));

            _color_picker = new ColorPicker("color_picker");

            var source_rect_size_mult_slider = new SelectorSlider("source_rect_size_slider", AppLayout.Data.SelectorThumbSize, new[] {1, 2, 4, 8});

            _brush_size_slider =
                new SelectorSlider("brush_size_slider", AppLayout.Data.SelectorThumbSize, new[] {1, 2, 4});

            _sprite_sheet_editor.SetSpriteSheet(sprite_sheet);
            sprite_sheet_navigator.SetSpriteSheet(sprite_sheet);
            _sprite_sheet_editor.SetPaintColor(_color_picker.CurrentColor);

            left_vertical_container.Add(_brush_size_slider);
            left_vertical_container.Add(_sprite_id_label);
            left_vertical_container.Add(_sprite_sheet_editor);
            left_vertical_container.Add(tool_box);
            left_vertical_container.Add(_color_picker);

            right_vertical_container.Add(source_rect_size_mult_slider);
            right_vertical_container.Add(sprite_sheet_navigator);

            Add(horizontal_container);

            horizontal_container.Layout(Orientation.Horizontal, ContainerAlignment.Stretch, ContainerAlignment.Stretch, 10, 10);
            left_vertical_container.Layout(Orientation.Vertical, ContainerAlignment.Center, ContainerAlignment.Center, 0, 10);
            right_vertical_container.Layout(Orientation.Vertical, ContainerAlignment.Center, ContainerAlignment.Center, 0, 10);
           
            TypedMessager<byte>.On(MessageCodes.ColorPicked, OnColorPick);

            source_rect_size_mult_slider.OnChange += value =>
            {
                TypedMessager<int>.Emit(MessageCodes.SpriteNavigatorCursorSizeChanged, value);
            };

            _brush_size_slider.OnChange += value =>
            {
                TypedMessager<int>.Emit(MessageCodes.SpriteSheetEditorBrushSizeChanged, value);
            };

            TypedMessager<int>.On(MessageCodes.ToolTriggered, OnToolActionTriggered);

            TypedMessager<int>.On(MessageCodes.SpriteIdChanged, OnSpriteIdChanged);
        }

        private void OnSpriteIdChanged(int id)
        {
            _sprite_id_label.Text = $"#{id:000}";
        }

        private void OnToolActionTriggered(int action)
        {
            switch (action)
            {
                case (int)Tools.Pen:
                case (int)Tools.Fill:
                case (int)Tools.Select:
                    SetCurrentTool((Tools)action);
                    break;

                case (int)Actions.FlipH:
                    _sprite_sheet_editor.FlipH();
                    break;
                case (int)Actions.FlipV:
                    _sprite_sheet_editor.FlipV();
                    break;
                case (int)Actions.Rotate:
                    _sprite_sheet_editor.Rotate();
                    break;
                case (int)Actions.Clear:
                    _sprite_sheet_editor.ClearFrame();
                    break;
            }
        }


        private void SetCurrentTool(Tools tool)
        {
            _sprite_sheet_editor.SetCurrentTool(tool);
            Ui.SetVisible(_brush_size_slider, _sprite_sheet_editor.CurrentTool.UseVariableBrushSize);
        }

        private void OnColorPick(byte color)
        {
            _color_picker.SetColor(color);
        }

        public override void OnKeyDown(Key key)
        {
            switch (key)
            {
                case Key.D1:
                    _brush_size_slider.SetValue(1);
                    break;
                case Key.D2:
                    _brush_size_slider.SetValue(2);
                    break;
                case Key.D3:
                    _brush_size_slider.SetValue(4);
                    break;
            }
        }

    }
}
