
using System.Collections.Generic;

namespace BLIT64.Toolkit.Gui
{
    public partial class UI
    {
        public Widget HoveredWidget { get; private set; }

        public Widget ActiveWidget { get; private set; }

        public Widget InputFocusedWidget { get; private set; }

        public IGuiDrawer Drawer { get; set; } = new DefaultGuiDrawer();

        public bool DrawStats { get; set; } = false;

        private static int _zindex = 1;

        private readonly Dictionary<string, int> _map;

        private readonly Dictionary<string, List<Widget>> _toggle_groups;

        private readonly List<Widget> _widgets;

        private List<Widget> _updatable_widgets;

        private readonly Container _root;

        private int _last_mouse_x;
        private int _last_mouse_y;

        private int _mouse_x;
        private int _mouse_y;


        public UI()
        {
            Input.AddMouseDownListener(ProcessMouseDown);
            Input.AddMouseUpListener(ProcessMouseUp);
            Input.AddMouseMoveListener(ProcessMouseMove);
            Input.AddKeyDownListener(ProcessKeyDown);
            Input.AddKeyUpListener(ProcessKeyUp);

            _root = new Container("root", Game.Instance.Canvas.Width, Game.Instance.Canvas.Height);

            _map = new Dictionary<string, int>();

            _toggle_groups = new Dictionary<string, List<Widget>>();

            _widgets = new List<Widget>();

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

        public void SetProcess(Widget widget, bool process)
        {
            if (_updatable_widgets == null)
            {
                _updatable_widgets = new List<Widget>();
            }

            if (process)
            {
                if (_updatable_widgets.Contains(widget))
                {
                    _updatable_widgets.Add(widget);
                }
                else
                {
                    _updatable_widgets.Remove(widget);
                }
            }
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
                else
                {
                    widget.Hovered = false;
                }

                if (InputFocusedWidget == widget)
                {
                    SetInputFocus(widget, false);
                }
                else
                {
                    widget.HasInputFocus = false;
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

        public void Update()
        {
            if (_updatable_widgets == null)
            {
                return;
            }

            foreach (var updatable_widget in _updatable_widgets)
            {
                updatable_widget.Update();
            }
        }

        public void Draw(Canvas blitter)
        {
            _root.Draw(blitter, Drawer);

            if (DrawStats)
            {
                blitter.SetColor(Palette.BlackColor);
                blitter.Text(10, 10, $"Hovered: {HoveredWidget?.Id ?? "None"}", 1);
                blitter.Text(10, 20, $"Active: {ActiveWidget?.Id ?? "None"}", 1);
                blitter.Text(10, 30, $"Input Focused: {InputFocusedWidget?.Id ?? "None"}", 1);

                for (int i = 0; i < _widgets.Count; ++i)
                {
                    var w = _widgets[i];
                    blitter.Text(10,  50 + (i)*10, $"{w.Id} [ZIndex: {w.ZIndex}]", 1);
                }
            }
        }

        internal void AssignToggleGroup(Widget widget, string group)
        {
            if (group != null)
            {
                if (!_toggle_groups.TryGetValue(group, out _))
                {
                    _toggle_groups.Add(group, new List<Widget>());                
                }
                
                _toggle_groups[group].Add(widget);
            }
            else
            {
                List<string> empty_groups = new List<string>();

                foreach (var toggle_group in _toggle_groups)
                {
                    if (toggle_group.Value.Contains(widget))
                    {
                        toggle_group.Value.Remove(widget);

                        if (toggle_group.Value.Count == 0)
                        {
                            empty_groups.Add(toggle_group.Key);
                        }
                    }

                }

                foreach (var empty_group in empty_groups)
                {
                    _toggle_groups.Remove(empty_group);
                }

                empty_groups.Clear();
            }
            
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

            if (x == 0 && y == 0)
            {
                return;
            }

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

            ActiveWidget.ProcessMouseUp(button, _mouse_x - ActiveWidget.DrawX, _mouse_y - ActiveWidget.DrawY);

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

            HoveredWidget.Active = true;
            ActiveWidget = HoveredWidget;
            ActiveWidget.ProcessMouseDown(button, _mouse_x - ActiveWidget.DrawX, _mouse_y - ActiveWidget.DrawY);

            if (button == MouseButton.Left)
            {
                if (ActiveWidget.Toggable && ActiveWidget.ToggleGroup == null)
                {
                    ActiveWidget.On = !ActiveWidget.On;
                }
                else if (ActiveWidget.Toggable && ActiveWidget.ToggleGroup != null)
                {
                    ActiveWidget.On = true;
                    UpdateToggleGroup(ActiveWidget);
                }

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


        internal void UpdateToggleGroup(Widget widget)
        {
            if (widget.ToggleGroup == null)
            {
                return;
            }

            var toggle_group = _toggle_groups[widget.ToggleGroup];

            foreach (var widget_in_group in toggle_group)
            {
                if (widget_in_group != widget)
                {
                    widget_in_group.On = false;
                }
            }
        }
       
    }
}
