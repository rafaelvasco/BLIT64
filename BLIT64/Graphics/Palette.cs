
namespace BLIT64
{
    public class Palette
    {
        public static readonly int ColorCount = 64;

        public const byte TransparentColor = 0;
        public const byte WhiteColor = 1;
        public const byte BlackColor = 2;
        public const byte NoColor = 255;
        
        public string Name { get; private set;}

        public int Count => _colors.Length;
        protected Color[] _colors;

        public Palette(string name, Color[] colors)
        {
            Name = name;
            _colors = colors;
        }

        public byte MatchColor(byte r, byte g, byte b)
        {
            byte best_match = 0;
            var best_delta = 765;

            for (var i = 0; i < _colors.Length; ++i)
            {
                ref var palette_color = ref _colors[i];

                var delta = palette_color.Delta(r, g, b);

                if (delta < best_delta)
                {
                    best_delta = delta;
                    best_match = (byte)i;
                }
            }

            return best_match;
        }

        public ref Color Map(int index)
        {
            return ref _colors[index];
        }
    }
}
