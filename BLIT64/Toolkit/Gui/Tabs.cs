

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLIT64.Toolkit.Gui
{
    public class TabButton : Button
    {
        public int Index { get; }

        public delegate void TabButtonEventHandler(int index);

        public event TabButtonEventHandler OnSelect;

        public TabButton(int index, string id, string label, int width = DefaultWidth, int height = DefaultHeight) : base(id, label, width, height)
        {
            Index = index;
            OnClick += OnClickTab;
            CanHaveInputFocus = false;
        }

        public override void OnMouseDown(MouseButton button)
        {
            _label.OffsetY = 1;
        }

        public override void OnMouseUp(MouseButton button)
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

        public TabHeader(string id, int width, int height) : base(id, width, height)
        {
        }

        internal TabButton AddTab(string name)
        {
            var tab_id = _tab_count++;
            var tab_header = new TabButton(tab_id, Id + "_" + name, name, 50, Height-2);
            Add(tab_header);
            tab_header.X = tab_id * tab_header.Width;
            return tab_header;
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawTabHeader(blitter, this);

            DrawChildren(blitter, theme);
        }
    }

    public class Tabs : Container
    {
        private int _tab_index = 0;

        private readonly TabHeader _header;

        private readonly List<Panel> _tab_panels;

        public int TabHeaderHeight { get; set; } = 30;

        public Tabs(string id, int width, int height) : base(id, width, height)
        {
            _tab_panels = new List<Panel>();

            _header = new TabHeader(id + "header", Width, TabHeaderHeight);

            Add(_header);
        }

        public Panel AddTab(string title)
        {
            var tab = _header.AddTab(title);
            tab.OnSelect += TabOnSelect;

            var panel = new Panel(Id + "_panel", Width, Height - TabHeaderHeight)
            {
                Y = TabHeaderHeight,
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
