
using System.Collections.Generic;

namespace BLIT64
{
    public class SpriteSheet : Pixmap
    {
        protected readonly Rect[] Tiles;

        public List<string> SpriteNames { get; private set; }

        private Dictionary<string, int> _sprite_map;

        public int TileSize { get; }

        public int TileCount => Tiles.Length;

        public void MapNamedSprite(string name, int index)
        {
            if (_sprite_map == null)
            {
                _sprite_map = new Dictionary<string, int>();
                SpriteNames = new List<string>();
            }

            if (!_sprite_map.TryGetValue(name, out _))
            {
                SpriteNames.Add(name);

                _sprite_map.Add(name, index);
            }
        }

        private static Rect[] BuildTiles(int width, int height, int cell_size)
        {
            var cells_horiz = width / cell_size;
            var cells_vert = height / cell_size;
            var idx = 0;

            var tiles = new Rect[cells_horiz * cells_vert];
            for (var j = 0; j < cells_vert; ++j)
            {
                for (var i = 0; i < cells_horiz; ++i)
                {
                    tiles[idx++] = new Rect(i * cell_size, j * cell_size, cell_size, cell_size);
                }
            }

            return tiles;
        }

        internal SpriteSheet(int width, int height, int cell_size):base(width, height)
        {
            TileSize = cell_size;
            Tiles = SpriteSheet.BuildTiles(width, height, cell_size);
        }

        internal SpriteSheet(IReadOnlyList<byte> image_data, int width, int height, int cell_size) : base(image_data, width, height)
        {
            TileSize = cell_size;
            Tiles = SpriteSheet.BuildTiles(width, height, cell_size);
        }


        public ref readonly Rect this[int index] => ref Tiles[Calc.Clamp(index, 0, Tiles.Length - 1)];

        public int this[string name]
        {
            get
            {
                if (_sprite_map == null) return -1;

                if (_sprite_map.TryGetValue(name, out var result))
                {
                    return result;
                }

                return -1;
            }
        }
    }
}
