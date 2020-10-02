namespace BLIT64.Toolkit.Gui
{
    public abstract class Theme
    {
        public abstract void DrawButton(Blitter blitter, Button button);
        public abstract void DrawPanel(Blitter blitter, Panel panel);
        public abstract void DrawLabel(Blitter blitter, Label label);
        public abstract void DrawCheckbox(Blitter blitter, CheckBox checkbox);
        public abstract void DrawWindow(Blitter blitter, Window window);
        public abstract void DrawWindowCloseButton(Blitter blitter, Button button);
        public abstract void DrawTabHeader(Blitter blitter, TabHeader tab_header);
        public abstract void DrawListView(Blitter blitter, ListView list_view);

    }
}
