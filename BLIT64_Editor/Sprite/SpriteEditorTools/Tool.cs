using BLIT64;

namespace BLIT64_Editor
{
    public class ToolActionParams
    {
        public Blitter Blitter;
        public Pixmap Overlay;
        public SpriteSheet SpriteSheet;
        public Rect SourceRect;
        public int PaintX;
        public int PaintY;
        public int CursorX;
        public int CursorY;
        public MouseButton MouseButton;
        public byte PaintColor;
    }

    public abstract class Tool
    {
        protected int _brush_size = 1;

        public abstract int BrushSize { get; set; }

        public abstract bool UseVariableBrushSize { get; }

        public abstract void OnMouseDown(ToolActionParams @params);

        public abstract void OnMouseUp(ToolActionParams @params);

        public abstract void OnMouseMove(ToolActionParams @params);

        public abstract void OnKeyDown(Key key, ToolActionParams @params);

        public abstract void Update(ToolActionParams @params);

        public virtual void OnActivate() { }

        public virtual void OnDeactivate() { }
    }
}
