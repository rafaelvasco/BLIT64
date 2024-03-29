﻿
using BLIT64;

namespace BLIT64_Editor
{
    public class FillTool : Tool
    {

        public FillTool()
        {
            _brush_size = 1;
        }

        public override int BrushSize
        {
            get => _brush_size;
            set
            {

            }
        }
        public override bool UseVariableBrushSize => false;

        public override void OnMouseDown(ToolActionParams @params)
        {
            var canvas = @params.Canvas;
            var x = @params.PaintX;
            var y = @params.PaintY;
            var button = @params.MouseButton;
            var paint_color = @params.PaintColor;
            var sheet = @params.SpriteSheet;
            var source_rect = @params.SourceRect;

            canvas.SetTarget(sheet);
            canvas.Clip(source_rect.X, source_rect.Y, source_rect.W, source_rect.H);
            canvas.FloodFill(x, y, button == MouseButton.Left ? paint_color : (byte)0);
            canvas.Clip();
            canvas.SetTarget();
        }

        public override void OnMouseUp(ToolActionParams @params)
        {
        }

        public override void OnMouseMove(ToolActionParams @params)
        {
        }

        public override void OnKeyDown(Key key, ToolActionParams @params)
        {
        }

        public override void Update(ToolActionParams @params)
        {
        }
    }
}
