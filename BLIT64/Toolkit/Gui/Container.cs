using System;
using System.Collections.Generic;

namespace BLIT64.Toolkit.Gui
{
    public class Container : Widget
    {
        public List<Widget> Children { get; }

        public Container(string id, int width, int height) : base(id, width, height)
        {
            Children = new List<Widget>();
        }

        public void Add(Widget child)
        {
            child.Parent = this;

            Children.Add(child);
            Ui.Register(child);

            if (!this.Visible)
            {
                Ui.SetVisible(child, false);
            }
        }

        protected void DrawChildren(Blitter blitter, Theme theme)
        {
            foreach (var widget in Children)
            {
                if (!widget.Visible)
                {
                    continue;
                }

                widget.Draw(blitter, theme);
            }
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            DrawChildren(blitter, theme);
        }
    }
}
