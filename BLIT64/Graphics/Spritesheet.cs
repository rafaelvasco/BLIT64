
using System;

namespace BLIT64
{
    public class SpriteSheet : Pixmap
    {
        protected readonly Rect[] _tiles;

        internal SpriteSheet(byte[] image_data, int image_width, int image_height, int cell_width, int cell_height) : base(image_data, image_width, image_height)
        {
            int cells_horiz = this.Width / cell_width;
            int cells_vert = this.Height / cell_height;
            int idx = 0;

            _tiles = new Rect[cells_horiz * cells_vert];
            for (int i = 0; i < cells_horiz; ++i)
            {
                for (int j = 0; j < cells_vert; ++j)
                {
                    _tiles[idx++] = new Rect(i * cell_width, j * cell_height, cell_width, cell_height);
                }
            }
        }

        public ref Rect this[int index] => ref _tiles[Calc.Clamp(index, 0, _tiles.Length - 1)];
    }
}
