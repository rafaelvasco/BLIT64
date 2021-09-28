
using Microsoft.VisualBasic;

namespace BLIT64
{
    public enum ColorGroup
    {
        Group1 = 0,
        Group2 = 1,
        Group3 = 2,
        Group4 = 3,
    }

    public static class Palette
    {
        public const int NullColor = -1;

        public const int DefaultDarkColor = 0;

        public const int DefaultLightColor = 35;

        public const int DefaultRedColor = 56;

        public const int DefaultGreenColor = 45;

        public const int DefaultBlueColor = 31;

        public const int ColorCount = 64;

        private static readonly Color[] _colors;

        private static int _palette_scene_index;

        private const byte ColorKeyR = 255;

        private const byte ColorKeyG = 0;

        private const byte ColorKeyB = 255;

        static Palette()
        {
            _colors = new Color[]
            {
                new(0x050914),
                new(0x110524),
                new(0x3b063a),
                new(0x691749),
                new(0x9c3247),
                new(0xd46453),
                new(0xf5a15d),
                new(0xffcf8e),
                new(0xff7a7d),
                new(0xff417d),
                new(0xd61a88),
                new(0x94007a),
                new(0x42004e),
                new(0x220029), 
                new(0x100726),
                new(0x25082c),
                new(0x3d1132),
                new(0x73263d),
                new(0xbd4035),
                new(0xed7b39),
                new(0xffb84a),
                new(0xfff540),
                new(0xc6d831),
                new(0x77b02a),
                new(0x429058),
                new(0x2c645e),
                new(0x153c4a),
                new(0x052137),
                new(0x0e0421),
                new(0x0c0b42),
                new(0x032769),
                new(0x144491),
                new(0x488bd4),
                new(0x78d7ff),
                new(0xb0fff1),
                new(0xfaffff),
                new(0xc7d4e1),
                new(0x928fb8),
                new(0x5b537d),
                new(0x392946),
                new(0x24142c),
                new(0x0e0f2c),
                new(0x132243),
                new(0x1a466b),
                new(0x10908e),
                new(0x28c074),
                new(0x3dff6e),
                new(0xf8ffb8),
                new(0xf0c297),
                new(0xcf968c),
                new(0x8f5765),
                new(0x52294b),
                new(0x0f022e),
                new(0x35003b),
                new(0x64004c),
                new(0x9b0e3e),
                new(0xd41e3c),
                new(0xed4c40),
                new(0xff9757),
                new(0xd4662f),
                new(0x9c341a),
                new(0x691b22),
                new(0x450c28),
                new(0x2d002e),

                new(0x2f1e45), 
                new(0x6a1948), 
                new(0x901a4d), 
                new(0xc1264b), 
                new(0xdf3551), 
                new(0xee5959), 
                new(0xe48b47), 
                new(0xd86545), 
                new(0xbb4343), 
                new(0xa52c49), 
                new(0x8a1f40), 
                new(0x4e1829), 
                new(0x632337), 
                new(0x7a3243), 
                new(0x8f4747), 
                new(0xac6754), 
                new(0xbe8960), 
                new(0xcfa35f), 
                new(0xf5cf8a), 
                new(0xdb9d28), 
                new(0xa76609), 
                new(0x803d11), 
                new(0x444800), 
                new(0x5e5d0a), 
                new(0x817c14), 
                new(0xa7983a), 
                new(0xbfaf60), 
                new(0xc9e276), 
                new(0x90b53a), 
                new(0x5d9226), 
                new(0x0f6a2e), 
                new(0x0c403b), 
                new(0x16315e), 
                new(0x0f4d69), 
                new(0x1c707f), 
                new(0x269992), 
                new(0x6dc0b4), 
                new(0x75d9f2), 
                new(0x55a7d4), 
                new(0x3a8bc2), 
                new(0x2c5791), 
                new(0x183360), 
                new(0x7d2f7e), 
                new(0xa83690), 
                new(0xce4999), 
                new(0xf27dcd), 
                new(0xfbb8ff), 
                new(0xc783e9), 
                new(0xa750c4), 
                new(0x743c9f), 
                new(0x493277), 
                new(0x372466), 
                new(0x1a3349), 
                new(0x34495c), 
                new(0x45596b), 
                new(0x607884), 
                new(0x79999b), 
                new(0xe6eaea), 
                new(0xb9cbc7), 
                new(0x91a8a8), 
                new(0x6d7f85), 
                new(0x53606b), 
                new(0x3c4550), 
                new(0x222734),

                new(0x140e1e), 
                new(0x2d1a71), 
                new(0x3257be), 
                new(0x409def), 
                new(0x70dbff), 
                new(0xbfffff), 
                new(0x3e32d5), 
                new(0x6e6aff), 
                new(0xa6adff), 
                new(0xd8e0ff), 
                new(0x652bbc), 
                new(0xb44cef), 
                new(0xec8cff), 
                new(0xffcdff), 
                new(0x480e55), 
                new(0x941887), 
                new(0xe444c3), 
                new(0xff91e2), 
                new(0x190c12), 
                new(0x550e2b), 
                new(0xaf102e), 
                new(0xff424f), 
                new(0xff9792), 
                new(0xffd5cf), 
                new(0x491d1e), 
                new(0xaa2c1e), 
                new(0xf66d1e), 
                new(0xffae68), 
                new(0xffe1b5), 
                new(0x492917), 
                new(0x97530f), 
                new(0xdd8c00), 
                new(0xfbc800), 
                new(0xfff699), 
                new(0x0c101b), 
                new(0x0e3e12), 
                new(0x38741a), 
                new(0x6cb328), 
                new(0xafe356), 
                new(0xe4fca2), 
                new(0x0d384c), 
                new(0x177578), 
                new(0x00bc9f), 
                new(0x6becbd), 
                new(0xc9fccc), 
                new(0x353234), 
                new(0x665d5b), 
                new(0x998d86), 
                new(0xcdbfb3), 
                new(0xeae6da), 
                new(0x2f3143), 
                new(0x505d6d), 
                new(0x7b95a0), 
                new(0xa6cfd0), 
                new(0xdfeae4), 
                new(0x8d4131), 
                new(0xcb734d), 
                new(0xefaf79), 
                new(0x9c2b3b), 
                new(0xe45761), 
                new(0xffffff), 
                new(0x000000), 
                new(0xe4162b), 
                new(0xffff40),

                new(0x998276), 
                new(0xc4c484), 
                new(0xabd883), 
                new(0xa2f2bd), 
                new(0xb88488), 
                new(0xd1b182), 
                new(0xd4eb91), 
                new(0xccfcc4), 
                new(0x907699), 
                new(0xc484a4), 
                new(0xea8c79), 
                new(0xf2e5a2), 
                new(0x9a84b8), 
                new(0xd182ca), 
                new(0xeb91a8), 
                new(0xffddc4), 
                new(0x768d99), 
                new(0x8484c4), 
                new(0xc479ea), 
                new(0xf2a2d7), 
                new(0x84b8b4), 
                new(0x82a2d1), 
                new(0xa791eb), 
                new(0xfbc8f5), 
                new(0x7c957a), 
                new(0x84c4a4), 
                new(0x79d7ea), 
                new(0xa2aff2), 
                new(0xa2b884), 
                new(0x82d189), 
                new(0x91ebd4), 
                new(0xc9e5fa), 
                new(0xb8a784), 
                new(0xb9ca89), 
                new(0x91eb91), 
                new(0xc9fce9), 
                new(0x957686), 
                new(0xc49484), 
                new(0xeade7a), 
                new(0xc3f2a2), 
                new(0xb884af), 
                new(0xd1828f), 
                new(0xebbd91), 
                new(0xf7f9c4), 
                new(0x797699), 
                new(0xb484c4), 
                new(0xea79bb), 
                new(0xf2a9a2), 
                new(0x8495b8), 
                new(0x9d82d1), 
                new(0xea91eb), 
                new(0xffc8d4), 
                new(0x76958d), 
                new(0x84b4c4), 
                new(0x7982ea), 
                new(0xd1a2f2), 
                new(0x84b88d), 
                new(0x82d1c4), 
                new(0x91beeb), 
                new(0xd2c6fa), 
                new(0x969976), 
                new(0x94c484), 
                new(0x79eaa8), 
                new(0xa2ebf2),
            };
        }

        public static void SetColorGroup(ColorGroup scene)
        {
            _palette_scene_index = (int)scene * 64;
        }

        public static void ChangeColor(int index, Color color)
        {
            _colors[_palette_scene_index + index] = color;
        }

        public static int MatchColor(byte r, byte g, byte b)
        {
            if (r == ColorKeyR && g == ColorKeyG && b == ColorKeyB)
            {
                return NullColor;
            }

            int best_match = 0;
            var best_delta = 765;

            for (var i = 0; i < _colors.Length; ++i)
            {
                ref var palette_color = ref _colors[i];

                var delta = palette_color.Delta(r, g, b);

                if (delta < best_delta)
                {
                    best_delta = delta;
                    best_match = i;
                }
            }

            return best_match;
        }

        public static ref Color MapIndexToColor(int index)
        {
            return ref _colors[_palette_scene_index + index];
        }
    }
}
