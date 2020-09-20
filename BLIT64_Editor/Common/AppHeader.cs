using System;
using BLIT64;

namespace BLIT64_Editor.Common
{
    public class AppHeader : Component
    {
        public AppHeader(Blitter blitter, Rect area) : base(blitter, area)
        {
        }

        public override void OnMouseDown(MouseButton button, int x, int y)
        {
        }

        public override void OnMouseUp(MouseButton button, int x, int y)
        {
        }

        public override void OnMouseMove(int x, int y)
        {
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            var blitter = _blitter;

            blitter.Rect(_area.X, _area.Y, _area.W, _area.H, Palette.WhiteColor);
        }
    }
}
