
using System.Collections.Generic;

namespace BLIT64
{
    public class Font : SpriteSheet
    {
        internal Font(
            IReadOnlyList<byte> image_data, 
            int image_width, 
            int image_height, 
            int glyph_size) : base(
            image_data, 
            image_width, 
            image_height, 
            glyph_size)
        {
        }

        public int GlyphSize => TileSize;

        public ref Rect this[char character]
        {
            get
            {
                int char_code = character;
                int index = char_code - 32;

                if (index < 0 || index > Tiles.Length-1)
                {
                    index = 0;
                }

                return ref Tiles[index];
            }
        }
    }
}
