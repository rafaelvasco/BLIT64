using BLIT64;

namespace BLIT64_Editor
{
    public class ToolActionParams
    {
        public Blitter Blitter;
        public SpriteSheet SpriteSheet;
        public Rect SourceRect;
        public int X;
        public int Y;
        public int BrushSize;
        public MouseButton MouseButton;
        public byte PaintColor;
    }

    public abstract class Tool
    {
        public abstract void OnMouseDown(ToolActionParams @params);

        public abstract void OnMouseUp(ToolActionParams @params);

        public abstract void OnMouseMove(ToolActionParams @params);
    }
}
