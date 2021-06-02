using System.Collections.Generic;

namespace BLIT64.Toolkit.Gui
{
    public class TabButton : Button
    {
        public int Index { get; }

        public delegate void TabButtonEventHandler(int index);

        public event TabButtonEventHandler OnSelect;

        public TabButton(int index, string id, string label, int width , int height) : base(id, label, width, height)
        {
            Index = index;
            OnClick += OnClickTab;
            CanHaveInputFocus = false;
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
            _label.OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
            _label.OffsetY = 0;
        }

        private void OnClickTab()
        {
            OnSelect?.Invoke(this.Index);
        }
    }

    public class TabHeader : Container
    {
        private int _tab_count;
        private int _tab_pos_index;
        private readonly int _tab_width;


        public TabHeader(string id, int width, int height, int tab_width) : base(id, width, height)
        {
            _tab_width = tab_width;
        }

        internal TabButton AddTab(string name)
        {
            var tab_id = _tab_count++;
            var tab_header = new TabButton(tab_id, Id + "_" + name, name, _tab_width, Height)
            {
                ToggleGroup = Id + "_toggle_group",
                Toggable = true
            };

            if (Children.Count == 0)
            {
                tab_header.On = true;
            }

            Add(tab_header);

            tab_header.X = _tab_pos_index;

            _tab_pos_index = tab_header.X + tab_header.Width + 2;

            return tab_header;
        }

        public override void Draw(Canvas blitter, IGuiDrawer drawer)
        {
            DrawChildren(blitter, drawer);
        }
    }

    public class Tabs : Container
    {
        private int _tab_index = 0;

        private readonly TabHeader _header;

        private readonly List<Panel> _tab_panels;

        private readonly int _tab_header_height;

        public Tabs(string id, int width, int height, int tab_width, int tab_height) : base(id, width, height)
        {
            _tab_header_height = tab_height;

            _tab_panels = new List<Panel>();

            _header = new TabHeader(id + "header", _width, height: _tab_header_height, tab_width: tab_width);

            Add(_header);
        }

        public Panel AddTab(string title)
        {
            var tab = _header.AddTab(title);
            tab.OnSelect += TabOnSelect;

            var panel = new Panel(Id + "_panel", Width, Height - _tab_header_height)
            {
                Y = _tab_header_height,
                IgnoreInput = true,
                CanHaveInputFocus = false
            };

            Add(panel);
            _tab_panels.Add(panel);
            Ui.SetVisible(panel, _tab_panels.Count == 1);

            return panel;
        }

        private void TabOnSelect(int index)
        {   
            Ui.SetVisible(_tab_panels[_tab_index], false);

            _tab_index = index;

            Ui.SetVisible(_tab_panels[index], true);
        }
    }
}
