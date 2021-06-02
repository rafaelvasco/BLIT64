using System;
using System.Collections.Generic;
using BLIT64;
using BLIT64.Toolkit.Gui;
using BLIT64_Editor.Common;

namespace BLIT64_Editor
{

    public class ToolButton
    {
        public enum ToolButtonMode
        {
            Tool,
            Modifier
        }

        public Rect Area;
        public ToolButtonMode Mode;
        public int Action;
    }

    public class ToolSelector : Container
    {
        private readonly SpriteSheet _icons;
        private readonly Dictionary<int, ToolButton> _tool_buttons;
        private int _current_tool_index;
        private int _current_pressed_action_index = -1;

        private readonly int _down_arrow_sprite;

        private readonly int tool_tiles_count = 7;

        public ToolSelector(string id) : base(id, AppLayout.Data.ToolBoxWidth, AppLayout.Data.ToolBoxHeight)
        {
            _icons = Assets.Get<SpriteSheet>("spr_ed_icons");
            _tool_buttons = new Dictionary<int, ToolButton>();

            _down_arrow_sprite = _icons["DownArrow"];

            BuildToolButtons();
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            foreach (var (index, tool_button) in _tool_buttons)
            {
                if (!tool_button.Area.Contains(x, y)) continue;

                if (tool_button.Mode == ToolButton.ToolButtonMode.Tool)
                {
                    _current_tool_index = index;
                }
                else if (tool_button.Mode == ToolButton.ToolButtonMode.Modifier)
                {
                    _current_pressed_action_index = index;
                }
                
                TypedMessager<int>.Emit(MessageCodes.ToolTriggered, tool_button.Action);
            }
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _current_pressed_action_index = -1;
        }

        public override void OnMouseMove(int x, int y)
        {
        }

        private void BuildToolButtons()
        {
            var icons_scale = AppLayout.Data.ToolBoxIconsScale;
            var tile_size = (int)(_icons.TileSize * icons_scale);
            var icons_spacing = AppLayout.Data.ToolBoxIconsSpacing;
            var icons_area_width = (tile_size * tool_tiles_count) + (tool_tiles_count + 1) * icons_spacing;

            for (int i = 0; i < tool_tiles_count; ++i)
            {
                var tool_button = new ToolButton
                {
                    Area = new Rect(
                        Width / 2 - (icons_area_width / 2) + (i * tile_size) + (icons_spacing * i) + icons_spacing,
                        Height / 2 - (tile_size / 2),
                        tile_size,
                        tile_size
                    )
                    
                };

                var action_name = _icons.SpriteNames[i];

                if (Enum.TryParse(typeof(Tools), action_name, out var action_tool))
                {
                    tool_button.Mode = ToolButton.ToolButtonMode.Tool;
                    tool_button.Action = (int)(Tools)action_tool;
                }
                else if (Enum.TryParse(typeof(Actions), action_name, out var action_modifier))
                {
                    tool_button.Mode = ToolButton.ToolButtonMode.Modifier;
                    tool_button.Action = (int)(Actions)action_modifier;
                }
                else
                {
                    continue;
                }

                _tool_buttons.Add(i, tool_button);
            }
        }

        public override void Update()
        {
        }

        public override void Draw(Canvas blitter, IGuiDrawer drawer)
        {
            var icons_scale = AppLayout.Data.ToolBoxIconsScale;

            var idx = 0;

            var icon_shadow_offset = AppLayout.Data.ToolBoxIconsShadowOffset;

            blitter.SetSpriteSheet(_icons);

            foreach (var (index, tool_button) in _tool_buttons)
            {
                var icon_offset = 0;

                var rect = tool_button.Area;

                if (index == _current_pressed_action_index || index == _current_tool_index)
                {
                    icon_offset = icon_shadow_offset;
                }

                blitter.Sprite(
                    id: idx,
                    x: DrawX + rect.X,
                    y: DrawY + rect.Y + icon_shadow_offset,
                    tint: Palette.BlackColor,
                    scale: icons_scale
                );
                blitter.Sprite(
                    id: idx,
                    x: DrawX + rect.X,
                    y: DrawY + rect.Y + icon_offset,
                    tint: _current_tool_index == index ? (byte)10 : Palette.NoColor,
                    scale: icons_scale
                );

                ++idx;

                if (tool_button.Mode == ToolButton.ToolButtonMode.Tool && _current_tool_index == index)
                {
                    blitter.Sprite(
                        id:_down_arrow_sprite,  
                        x:DrawX + rect.X, 
                        y:DrawY - AppLayout.Data.ToolBoxPointerMargin + 2, 
                        tint: Palette.BlackColor, 
                        scale: icons_scale);

                    blitter.Sprite(
                        id:_down_arrow_sprite,  
                        x:DrawX + rect.X, 
                        y:DrawY - AppLayout.Data.ToolBoxPointerMargin, 
                        tint: 10, 
                        scale: icons_scale);
                }
            }
        }
    }
}
