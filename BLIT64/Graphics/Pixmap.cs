
using System.Collections.Generic;

namespace BLIT64
{
    public class Pixmap : GameAsset
    {
        public int Width { get; }

        public int Height { get; }

        public int[] Colors;

        public Pixmap(int width, int height)
        {
            Width = width;
            Height = height;

            Colors = new int[width*height];

            for (int i = 0; i < Colors.Length; ++i)
            {
                Colors[i] = 0;
            }
        }

        public Pixmap(IReadOnlyList<byte> image_data, int width, int height)
        {
            Width = width;

            Height = height;

            var this_colors_index = 0;

            Colors = new int[width * height];

            for (int i = 0; i < image_data.Count; i+=4)
            {
                var cr = image_data[i];
                var cg = image_data[i+1];
                var cb = image_data[i+2];
                var matched_color_code = Palette.MatchColor(cr, cg, cb);
                Colors[this_colors_index++] = matched_color_code;
            }
        }

        public int GetColorAt(int x, int y)
        {
            var colors = this.Colors;

            var idx = x + y * Width;

            return colors[idx];
        }

        protected override void FreeManaged()
        {
            Colors = null;
        }
    }
}
