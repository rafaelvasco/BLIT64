
namespace BLIT64.Toolkit.Gui
{
    public class Panel : Container
    {
        public Panel(string id, int width, int height) : base(id, width, height)
        {
        }

        public override void Draw(Canvas canvas, IGuiDrawer drawer)
        {
            drawer.DrawPanel(canvas, this);

            DrawChildren(canvas, drawer);
        }
    }
}
