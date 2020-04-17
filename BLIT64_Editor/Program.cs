using BLIT64;

namespace BLIT64_Editor
{
    internal class Program
    {
        private static void Main()
        {
            using var game = new Game(
                title: "BLIT64 Editor",
                display_width: 1280,
                display_height: 720,
                render_surface_width: 640,
                render_surface_height: 360);
            game.Run(scene: new Editor());
        }
    }
}
