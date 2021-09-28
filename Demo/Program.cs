
using BLIT64;

namespace Demo
{
    internal class Program
    {
        private static void Main()
        {
            using var game = new Game("BLIT64 Demo", 640, 360);
            game.Run(new Scene3());
        }
    }
}
