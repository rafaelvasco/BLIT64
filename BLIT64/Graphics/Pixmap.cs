
namespace BLIT64
{
    public class Pixmap : GameAsset
    {
        public int Width { get; }

        public int Height { get; }

        public byte[] Colors;

        public Pixmap(int width, int height)
        {
            Width = width;
            Height = height;

            Colors = new byte[width*height];

            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] = 0;
            }
        }

        public Pixmap(byte[] colors, int width, int height)
        {
            Width = width;

            Height = height;

            var current_palette = Game.Instance.CurrentPalette;

            var this_colors_index = 0;

            Colors = new byte[width * height];

            for (int i = 0; i < colors.Length; i+=4)
            {
                var cr = colors[i];
                var cg = colors[i+1];
                var cb = colors[i+2];
                var matched_color_index = current_palette.MatchColor(cr, cg, cb);
                Colors[this_colors_index++] = matched_color_index;
            }
        }

        public byte GetColorAt(int x, int y)
        {
            var colors = this.Colors;

            var idx = x + y * Width;

            return colors[idx];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Colors = null;
            }
        }
    }
}
