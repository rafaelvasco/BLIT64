using System.Collections.Generic;
using BLIT64;

namespace BLIT64_Editor
{
    public class ToolButton
    {
        public Rect Area;
        public int Index;
        public Actions ActionType;
    }

    public class ToolSelector : Component
    {
        private readonly SpriteSheet _icons;
        private readonly SpriteEditorLayout _layout;
        private readonly Dictionary<int, ToolButton> _tool_buttons;
        private int _current_tool_index = 0;
        private int _current_pressed_action_index = -1;

        public ToolSelector(SpriteEditorLayout layout, Blitter blitter, Rect area) : base(blitter, area)
        {
            _layout = layout;
            _icons = Assets.Get<SpriteSheet>("spr_ed_icons");
            _tool_buttons = new Dictionary<int, ToolButton>();
            BuildButtonRects();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            foreach (var (index, tool_button) in _tool_buttons)
            {
                if (!tool_button.Area.Contains(x, y)) continue;

                if (tool_button.ActionType == Actions.ChangeTool)
                {
                    _current_tool_index = index;
                }
                else if (tool_button.ActionType == Actions.ModifySprite)
                {
                    _current_pressed_action_index = index;
                }
                
                Emit((int)tool_button.ActionType, tool_button.Index);
            }
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _current_pressed_action_index = -1;
        }

        public override void OnMouseMove(int x, int y)
        {
        }

        private void BuildButtonRects()
        {
            var icons_scale = _layout.ToolBoxIconsScale;
            var tile_size = (int)(_icons.TileSize * icons_scale);
            var icons_spacing = _layout.ToolBoxIconsSpacing;
            var icons_area_width = (tile_size * _icons.TileCount) + (_icons.TileCount + 1) * icons_spacing;

            for (int i = 0; i < _icons.TileCount-1; ++i)
            {
                var tool_button = new ToolButton
                {
                    Area = new Rect(
                        _area.W / 2 - (icons_area_width / 2) + (i * tile_size) + icons_spacing * i + icons_spacing,
                        _area.H / 2 - (tile_size / 2),
                        tile_size,
                        tile_size
                    ),
                    Index = i,
                    //TODO: Parameterize ActionType
                    ActionType = i < 3 ? Actions.ChangeTool : Actions.ModifySprite
                };

                //TODO:

                _tool_buttons.Add(i, tool_button);
            }
        }

        public override void Draw()
        {
            var blitter = _blitter;

            var icons_scale = _layout.ToolBoxIconsScale;

            var idx = 0;

            var icon_shadow_offset = _layout.ToolBoxIconsShadowOffset;

            foreach (var (index, tool_button) in _tool_buttons)
            {
                var icon_offset = 0;

                var rect = tool_button.Area;

                if (index == _current_pressed_action_index || index == _current_tool_index)
                {
                    icon_offset = icon_shadow_offset;
                }

                blitter.Sprite(
                    _icons,
                    id: idx,
                    x: _area.X + rect.X,
                    y: _area.Y + rect.Y + icon_shadow_offset,
                    tint: Palette.BlackColor,
                    scale: icons_scale
                );
                blitter.Sprite(
                    _icons,
                    id: idx,
                    x: _area.X + rect.X,
                    y: _area.Y + rect.Y + icon_offset,
                    tint: _current_tool_index == index ? (byte)46 : Palette.NoColor,
                    scale: icons_scale
                );

                ++idx;

                if (tool_button.ActionType == Actions.ChangeTool && _current_tool_index == index)
                {
                    blitter.Sprite(
                        _icons, 
                        id:_icons.TileCount-1,  
                        x:_area.X + rect.X, 
                        y:_area.Y - _layout.ToolBoxPointerMargin + 2, 
                        tint: Palette.BlackColor, 
                        scale: icons_scale);

                    blitter.Sprite(
                        _icons, 
                        id:_icons.TileCount-1,  
                        x:_area.X + rect.X, 
                        y:_area.Y - _layout.ToolBoxPointerMargin, 
                        tint: 46, 
                        scale: icons_scale);
                }
            }
        }
    }
}
