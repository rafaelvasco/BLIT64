namespace BLIT64.Toolkit.Gui
{
    public interface IGuiDrawer
    {
        public void DrawButton(Canvas blitter, Button button);
        public void DrawCheckbox(Canvas blitter, CheckBox check_box);
        public void DrawLabel(Canvas blitter, Label label);
        public void DrawListView(Canvas blitter, ListView list_view);
        public void DrawPanel(Canvas blitter, Panel panel);
        public void DrawSelectorSlider(Canvas blitter, SelectorSlider selector_slider);
        public void DrawTabs(Canvas blitter, Tabs tabs);
        public void DrawWindow(Canvas blitter, Window window);

    }
}
