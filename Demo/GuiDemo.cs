using System;
using BLIT64;
using BLIT64.Toolkit.Gui;

namespace Demo
{
    class GuiDemo : Scene
    {
        private UI ui;
        public override void Load()
        {
            ui = new UI()
            {
                DrawStats = true
            };

            var tabs = new Tabs("tabs", Game.Width, Game.Height);

            var panel1 = tabs.AddTab("tab1");
            var panel2 = tabs.AddTab("tab2");

            var button_panel_1 = new Button("button_panel_1", "Open Window", 160);

            var list_view = new ListView("list_view", 250, 200);

            list_view.AddItem("Item1", null);
            list_view.AddItem("Item2", null);
            list_view.AddItem("Item3", null);
            list_view.AddItem("Item4", null);
            list_view.AddItem("Item1", null);
            list_view.AddItem("Item2", null);
            list_view.AddItem("Item3", null);
            list_view.AddItem("Item4", null);
            list_view.AddItem("Item1", null);
            list_view.AddItem("Item2", null);
            list_view.AddItem("Item3", null);
            list_view.AddItem("Item4", null);
            list_view.AddItem("Item1", null);
            list_view.AddItem("Item2", null);
            list_view.AddItem("Item3", null);

            list_view.OnSelect += item => { Console.WriteLine($"Selected: {item.Label}"); };

            panel1.Add(button_panel_1);

            panel2.Add(list_view);

            button_panel_1.Align(Alignment.Center);

            list_view.Align(Alignment.Center);

            ui.Add(tabs);

            var window = new Window("window", 300, 300);

            ui.SetVisible(window, false);

            ui.Add(window);

            button_panel_1.OnClick += () => { window.ShowAndFocus(); };
        }

        public override void Update()
        {
            if (Input.KeyPressed(Key.D))
            {
                ui.DrawStats = !ui.DrawStats;
            }
        }

        public override void Draw(Blitter blitter)
        {
            blitter.Clear(8);

            ui.Draw(blitter);
        }
    }
}
