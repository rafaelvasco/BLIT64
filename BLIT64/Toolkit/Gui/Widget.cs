
using System;

namespace BLIT64.Toolkit.Gui
{
    public abstract class Widget : IComparable<Widget>
    {
        public delegate void WidgetMouseEventHandler();

        public event WidgetMouseEventHandler OnClick;
        public event WidgetMouseEventHandler OnPress;
        public event WidgetMouseEventHandler OnRelease;

        public static UI Ui { get; internal set; }

        private static int _global_uid;

        internal int Uid;

        internal int ZIndex;

        public string Id { get; }

        public int X { get; set; }
        public int Y { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public bool Visible { get; internal set; } = true;

        public bool IgnoreInput { get; set; } = false;

        public bool Hovered { get; internal set; }

        public bool Active { get; internal set; }

        public bool Draggable { get; set; }

        public bool Dragging { get; internal set; }

        public bool HasInputFocus { get; internal set; }

        public bool CanHaveInputFocus { get; set; } = false;

        public bool BubbleEventsToParent { get; set; } = false;

        internal int DrawX => Parent?.DrawX + X + OffsetX ?? X + OffsetX;
        internal int DrawY => Parent?.DrawY + Y + OffsetY ?? Y + OffsetY;

        public Rect GlobalGeometry => new Rect(DrawX, DrawY, Width, Height);

        public Container Parent { get; internal set; }

        public void ShowAndFocus()
        {
            if (this.Visible)
            {
                return;
            }

            Ui.SetVisible(this, true);
            Ui.SetInputFocus(this, true);
            Align(Alignment.Center);
        }

        protected Widget(string id, int width, int height)
        {
            Id = id;
            Uid = ++_global_uid;
            X = 0;
            Y = 0;
            Width = width;
            Height = height;
        }

        internal void ProcessMouseDown(MouseButton button)
        {
            OnMouseDown(button);

            if (Hovered)
            {
                OnPress?.Invoke();
            }
        }

        internal void ProcessMouseUp(MouseButton button)
        {
            OnMouseUp(button);
            if (Hovered)
            {
                OnClick?.Invoke();
                OnRelease?.Invoke();
            }

        }

        internal void ProcessMouseMove(int x, int y)
        {
            OnMouseMove(x, y);
        }

        internal void ProcessKeyDown(Key key)
        {
            OnKeyDown(key);

            Parent?.ProcessKeyDown(key);
        }

        internal void ProcessKeyUp(Key key)
        {
            OnKeyUp(key);

            Parent?.ProcessKeyUp(key);
        }

        internal void ProcessMouseEnter()
        {
            OnMouserEnter();
        }

        internal void ProcessMouseLeave()
        {
            OnMouseExit();
        }

        public virtual void OnMouseDown(MouseButton button) {}

        public virtual void OnMouseUp(MouseButton button) {}
        public virtual void OnMouseMove(int x, int y) {}
        public virtual void OnKeyDown(Key key) {}
        public virtual void OnKeyUp(Key key) {}
        public virtual void OnMouserEnter() {}
        public virtual void OnMouseExit() {}

        public override string ToString()
        {
            return Id;
        }

        public void Align(Alignment alignment, int margin_top=0, int margin_left=0, int margin_right=0, int margin_bottom=0)
        {
            switch (alignment)
            {
                case Alignment.TopLeft:

                    this.X = margin_left;
                    this.Y = margin_top;

                    break;
                case Alignment.Top:

                    this.X = Parent.Width / 2 - this.Width / 2;
                    this.Y = margin_top;

                    break;
                case Alignment.TopRight:

                    this.X = Parent.Width - this.Width - margin_right;
                    this.Y = margin_top;

                    break;
                case Alignment.Left:

                    this.X = margin_left;
                    this.Y = Parent.Height / 2 - this.Height / 2;

                    break;
                case Alignment.Center:

                    this.X = Parent.Width / 2 - this.Width / 2;
                    this.Y = Parent.Height / 2 - this.Height / 2;

                    break;
                case Alignment.Right:

                    this.X = Parent.Width - this.Width - margin_right;
                    this.Y = Parent.Height / 2 - this.Height / 2;

                    break;
                case Alignment.BottomLeft:

                    this.X = margin_left;
                    this.Y = Parent.Height - this.Height - margin_bottom;

                    break;
                case Alignment.Bottom:

                    this.X = Parent.Width / 2 - this.Width / 2;
                    this.Y = Parent.Height - this.Height - margin_bottom;

                    break;
                case Alignment.BottomRight:

                    this.X = Parent.Width - this.Width - margin_right;
                    this.Y = Parent.Height - this.Height - margin_bottom;

                    break;
            }
        }

        public abstract void Draw(Blitter blitter, Theme theme);

        public int CompareTo(Widget other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Uid.CompareTo(other.Uid);
        }
    }
}
