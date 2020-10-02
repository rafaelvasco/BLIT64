
namespace BLIT64.Toolkit.Gui
{
    public class Panel : Container
    {
        public Panel(string id, int width, int height) : base(id, width, height)
        {
        }

        public override void Draw(Blitter blitter, Theme theme)
        {
            theme.DrawPanel(blitter, this);

            DrawChildren(blitter, theme);
        }
    }
}
