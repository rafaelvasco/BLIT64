using BLIT64;

namespace BLIT64_Editor
{
    public abstract class Component
    {
        public ref Rect Area => ref _area;

        protected Blitter _blitter;
        protected Rect _area;

        public bool MouseOver(int x, int y)
        {
            return _area.Contains(x, y);
        }

        protected Component(Blitter blitter, Rect area)
        {
            _blitter = blitter;
            _area = area;
        }


        public abstract void OnMouseDown(MouseButton button, int x, int y);
        public abstract void OnMouseUp(MouseButton button, int x, int y);
        public abstract void OnMouseMove(int x, int y);

        public virtual void OnKeyDown(Key key) {}

        public virtual void OnKeyUp(Key key) {}

        public virtual void OnMouseEnter() {}
        public virtual void OnMouseLeave() {}
        public abstract void Draw();
    }
}
