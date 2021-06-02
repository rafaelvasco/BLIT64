using BLIT64;
using BLIT64.Toolkit.Gui;

namespace BLIT64_Editor
{
    public class App : Scene
    {
        private readonly UI _gui;

        public App()
        {
            _gui = new UI();

            AppLayout.Load();

            var tabs = new Tabs("section_nav", Game.Canvas.Width, Game.Canvas.Height, AppLayout.Data.SectionSelectorTabWidth, AppLayout.Data.SectionSelectorTabHeight);

            var sprite_tab = tabs.AddTab("SPRITE");
            var map_tab = tabs.AddTab("MAP");

            var sprite_editor = new SpriteEditor("sprite_editor", sprite_tab.Width, sprite_tab.Height);

            sprite_tab.Add(sprite_editor);

            sprite_editor.Align(Alignment.Center);

            _gui.Add(tabs);
        }

        public override void Load()
        {
            
        }

        public override void Update()
        {
            _gui.Update();
        }

        public override void Draw(Canvas blitter)
        {
            _gui.Draw(blitter);
        }
    }
}
