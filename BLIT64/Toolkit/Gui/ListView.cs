﻿
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace BLIT64.Toolkit.Gui
{
    public class ListItem
    {
        public string Label { get; set; }

        public object Value { get; set; }
    }

    public class ListView : Widget
    {
        public delegate void ListViewSelectHandler(ListItem item);

        public event ListViewSelectHandler OnSelect;

        public List<ListItem> Items { get; }

        public ListItem SelectedItem { get; private set; }

        public int RowHeight { get; set; } = 20;

        public int ScrollThumbWidth { get; set; } = 20;

        internal int HoveredIndex { get; private set; } = -1;

        internal int SelectedIndex { get; private set; } = -1;

        internal Rect ScrollThumbRect { get; private set; } = Rect.Empty;

        internal int TranslateY { get; private set; }

        private int _max_translate_y;

        private readonly int _max_visible_rows;

        private bool _scrolling;

        private int _mouse_x;

        private int _mouse_y;

        private int _last_mouse_y;


        public ListView(string id, int width, int height) : base(id, width, height)
        {
            Items = new List<ListItem>();

            _max_visible_rows = Height / RowHeight;
        }

        public void AddItem(string label, object value)
        {
            var item = new ListItem()
            {
                Label = label,
                Value = value
            };

            Items.Add(item);

            UpdateScroll();
        }

        public override void OnMouseMove(int x, int y)
        {
            _mouse_x = x;
            _mouse_y = y;

            if (_scrolling)
            {
                HoveredIndex = -1;

                var delta_y = _mouse_y - _last_mouse_y;

                TranslateY += delta_y;

                if (TranslateY < 0)
                {
                    TranslateY = 0;
                }

                if (TranslateY > _max_translate_y)
                {
                    TranslateY = _max_translate_y;
                }

                _last_mouse_y = _mouse_y;
            }
            else
            {
                if (ScrollThumbRect.Contains(_mouse_x, _mouse_y))
                {
                    HoveredIndex = -1;
                }
                else
                {
                    HoveredIndex = (y + TranslateY) / RowHeight;

                    if (HoveredIndex > Items.Count-1 || HoveredIndex < 0)
                    {
                        HoveredIndex = -1;
                    }
                }
            }
            
        }

        public override void OnMouseDown(MouseButton button)
        {
            if (ScrollThumbRect.Contains(_mouse_x, _mouse_y))
            {
                _scrolling = true;

                _last_mouse_y = _mouse_y;
            }
            else
            {
                if (HoveredIndex > -1)
                {
                    if (HoveredIndex == SelectedIndex)
                    {
                        SelectedIndex = -1;
                    }
                    else
                    {
                        SelectedIndex = HoveredIndex;

                        SelectedItem = Items[SelectedIndex];
                        OnSelect?.Invoke(SelectedItem);
                    }
                }
            }
        }


        public override void OnMouseUp(MouseButton button)
        {
            if (_scrolling)
            {
                _scrolling = false;
            }
        }


        public override void OnMouseExit()
        {
            HoveredIndex = -1;
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawListView(blitter, this);
        }

        private void UpdateScroll()
        {
            if (Items.Count <= _max_visible_rows)
            {
                ScrollThumbRect = Rect.Empty;
            }
            else
            {
                var thumb_height = Height - ((Items.Count - _max_visible_rows)) * RowHeight;

                _max_translate_y = Height - thumb_height;

                ScrollThumbRect = new Rect(Width-ScrollThumbWidth,  0, ScrollThumbWidth, thumb_height);
            }
        }
    }
}
