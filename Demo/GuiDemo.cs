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
                DrawStats = false
            };

            Canvas.DrawDebugInfo = false;

            var tabs = new Tabs("tabs", Game.Canvas.Width, Game.Canvas.Height, 200, 20);

            var panel1 = tabs.AddTab("Window");
            var panel2 = tabs.AddTab("List View");
            var panel3 = tabs.AddTab("Layout");

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

            var layout_container = new Container("layout_container", 300, 300);

            layout_container.Add(new Button("layout_container_b1", "B1", 50));
            layout_container.Add(new Button("layout_container_b2", "B3", 50));
            layout_container.Add(new Button("layout_container_b2", "B3", 50));

            layout_container.Layout(Orientation.Vertical, ContainerAlignment.Stretch, ContainerAlignment.End, 10,10);

            layout_container.DebugColor = 10;

            panel1.Add(button_panel_1);

            panel2.Add(list_view);

            panel3.Add(layout_container);

            layout_container.Align(Alignment.Center);

            button_panel_1.Align(Alignment.Center);

            list_view.Align(Alignment.Center);

            ui.Add(tabs);

            var window = new Window("window", 300, 300);

            ui.SetVisible(window, false);

            ui.Add(window);

            button_panel_1.OnClick += () => { window.ShowAndFocus(); };

            ui.Draw(Canvas);
        }

        public override void Update()
        {
            if (Input.KeyPressed(Key.D))
            {
                ui.DrawStats = !ui.DrawStats;
                Canvas.DrawDebugInfo = !Canvas.DrawDebugInfo;
            }
        }

        public override void Draw(Canvas canvas)
        {
            ui.Draw(canvas);
        }
    }
}
