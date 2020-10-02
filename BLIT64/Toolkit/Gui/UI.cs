
using System.Collections.Generic;

namespace BLIT64.Toolkit.Gui
{
    public class UI
    {
        public Widget HoveredWidget { get; private set; }

        public Widget ActiveWidget { get; private set; }

        public Widget InputFocusedWidget { get; private set; }

        public bool DrawStats { get; set; } = false;

        private static int _zindex = 1;

        private readonly Dictionary<string, int> _map;

        private readonly List<Widget> _widgets;

        private readonly Container _root;

        private int _last_mouse_x;
        private int _last_mouse_y;

        private int _mouse_x;
        private int _mouse_y;

        private Theme _gui_theme;
        private readonly Theme _default_theme;

        public Theme GuiTheme
        {
            get => _gui_theme;
            set => _gui_theme = value ?? _default_theme;
        }

        public UI()
        {
            Input.AddMouseDownListener(ProcessMouseDown);
            Input.AddMouseUpListener(ProcessMouseUp);
            Input.AddMouseMoveListener(ProcessMouseMove);
            Input.AddKeyDownListener(ProcessKeyDown);
            Input.AddKeyUpListener(ProcessKeyUp);

            _root = new Container("root", Game.Instance.Width, Game.Instance.Height);

            _map = new Dictionary<string, int>();

            _widgets = new List<Widget>();

            _default_theme = new DefaultTheme();

            _gui_theme = _default_theme;

            Widget.Ui = this;

        }

        public Widget this[string id]
        {
            get
            {
                if (_map.TryGetValue(id, out var index))
                {
                    return _widgets[index];
                }

                return null;
            }
        }

        public void Add(Widget widget)
        {
            if (Register(widget))
            {
                _root.Add(widget);
            }

            RecalculateZIndices(_root);
            ReorderWidgets();
        }

        public void SetVisible(string widget_name, bool visible)
        {
            if (_map.TryGetValue(widget_name, out int widget_index))
            {
                SetVisible(_widgets[widget_index], visible);
            }
        }

        public void SetVisible(Widget widget, bool visible)
        {
            widget.Visible = visible;

            if (visible)
            {
                if (widget is Container container)
                {
                    foreach (var container_child in container.Children)
                    {
                        SetVisible(container_child, true);
                    }
                }
            }

            else
            {
                if (HoveredWidget != null && HoveredWidget == widget)
                {
                    SetHovered(widget, false);
                }

                if (InputFocusedWidget == widget)
                {
                    SetInputFocus(widget, false);
                }

                if (widget is Container container)
                {
                    foreach (var container_child in container.Children)
                    {
                        SetVisible(container_child, false);
                    }
                }
            }

            ProcessMouseMove();
        }

        internal bool Register(Widget widget)
        {
            if (!_widgets.Contains(widget))
            {
                _widgets.Add(widget);
                RecalculateZIndices(_root);
                ReorderWidgets();
                return true;
            }

            return false;
        }

        internal void SetInputFocus(Widget widget, bool focus)
        {
            widget.HasInputFocus = focus;

            if (!focus)
            {
                if (widget == InputFocusedWidget)
                {
                    InputFocusedWidget = null;
                }
            }
            else
            {
                InputFocusedWidget = widget;
            }
        }

        internal void SetHovered(Widget widget, bool hovered)
        {
            if (hovered)
            {
                HoveredWidget = widget;
                HoveredWidget.Hovered = true;
            }
            else
            {
                widget.Hovered = false;
                HoveredWidget = null;
            }
        }

        private void RecalculateZIndices(Widget widget)
        {
            _zindex = -1;
            LoopRecursiveZIndex(widget);
        }

        private void LoopRecursiveZIndex(Widget widget)
        {
            widget.ZIndex = _zindex++;

            if (widget is Container container)
            {
                foreach (var container_child in container.Children)
                {
                    LoopRecursiveZIndex(container_child);
                }
            }
        }

        private void ReorderWidgets()
        {
            _widgets.Sort((widget1, widget2) => widget2.ZIndex.CompareTo(widget1.ZIndex));

            for (int i = 0; i < _widgets.Count; ++i)
            {
                _map[_widgets[i].Id] = i;
            }
        }

        private void ProcessKeyUp(Key key)
        {
            InputFocusedWidget?.ProcessKeyUp(key);
        }

        private void ProcessKeyDown(Key key)
        {
            InputFocusedWidget?.ProcessKeyDown(key);
        }

        private void ProcessMouseMove()
        {
            var (x, y) = Input.MousePos;

            _mouse_x = x;
            _mouse_y = y;

            foreach (var widget in _widgets)
            {
                if (widget.IgnoreInput || !widget.Visible)
                {
                    continue;
                }

                if (widget.GlobalGeometry.Contains(x, y))
                {
                    if (HoveredWidget != null && HoveredWidget != widget && HoveredWidget.Hovered)
                    {
                        HoveredWidget.Hovered = false;
                        HoveredWidget.ProcessMouseLeave();
                    }

                    var to_be_hovered = widget;

                    if (widget.BubbleEventsToParent && widget.Parent != null)
                    {
                        to_be_hovered = widget.Parent;
                    }

                    if (!to_be_hovered.Hovered)
                    {
                        widget.ProcessMouseEnter();
                    }

                    SetHovered(to_be_hovered, true);

                    break;
                }
                else if (widget.Hovered && widget == HoveredWidget)
                {
                    widget.ProcessMouseLeave();

                    SetHovered(widget, false);
                }
            }

            if (ActiveWidget != null)
            {
                if (!ActiveWidget.Draggable)
                {
                    ActiveWidget.ProcessMouseMove(x-ActiveWidget.DrawX, y-ActiveWidget.DrawY);
                }
                else if (ActiveWidget.Dragging)
                {
                    var dx = _mouse_x - _last_mouse_x;
                    var dy = _mouse_y - _last_mouse_y;

                    ActiveWidget.X += dx;
                    ActiveWidget.Y += dy;
                }
            }
            else
            {
                HoveredWidget?.ProcessMouseMove(x-HoveredWidget.DrawX, y-HoveredWidget.DrawY);
            }

            _last_mouse_x = _mouse_x;
            _last_mouse_y = _mouse_y;
        }

        private void ProcessMouseUp(MouseButton button)
        {
            if (ActiveWidget == null)
            {
                return;
            }

            if (ActiveWidget.Draggable)
            {
                ActiveWidget.Dragging = false;
            }

            ActiveWidget.ProcessMouseUp(button);
            ActiveWidget.Active = false;
            ActiveWidget = null;
        }

        private void ProcessMouseDown(MouseButton button)
        {
            if (HoveredWidget == null)
            {
                InputFocusedWidget = null;
                return;
            }

            if (button == MouseButton.Left)
            {
                HoveredWidget.Active = true;
                ActiveWidget = HoveredWidget;
                ActiveWidget.ProcessMouseDown(button);

                if (InputFocusedWidget != null)
                {
                    InputFocusedWidget.HasInputFocus = false;
                }

                if (ActiveWidget.CanHaveInputFocus)
                {
                    InputFocusedWidget = ActiveWidget;
                    InputFocusedWidget.HasInputFocus = true;
                }

                if (ActiveWidget.Draggable)
                {
                    ActiveWidget.Dragging = true;
                }
            }

            _last_mouse_x = _mouse_x;
            _last_mouse_y = _mouse_y;
        }

        public void Draw(Blitter blitter)
        {
            _root.Draw(blitter, _gui_theme);

            if (DrawStats)
            {
                blitter.Text(10, 10, $"Hovered: {HoveredWidget?.Id ?? "None"}", 1, Palette.BlackColor);
                blitter.Text(10, 20, $"Active: {ActiveWidget?.Id ?? "None"}", 1, Palette.BlackColor);
                blitter.Text(10, 30, $"Input Focused: {InputFocusedWidget?.Id ?? "None"}", 1, Palette.BlackColor);

                for (int i = 0; i < _widgets.Count; ++i)
                {
                    var w = _widgets[i];
                    blitter.Text(10,  50 + (i)*10, $"{w.Id} [ZIndex: {w.ZIndex}]", 1, Palette.BlackColor);
                }
            }
        }
    }
}
