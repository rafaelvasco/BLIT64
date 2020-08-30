
using System.Collections.Generic;

namespace BLIT64_Common
{
    public class AssetsPak
    {
        public int AssetsCount { get; private set; }
        public List<SpriteSheetData> SpriteSheets;
        public List<FontData> Fonts;

        public AssetsPak()
        {
            SpriteSheets = new List<SpriteSheetData>();
            Fonts = new List<FontData>();
        }

        internal void AddSheet(SpriteSheetData data)
        {
            SpriteSheets.Add(data);
            ++AssetsCount;
        }

        internal void AddFont(FontData data)
        {
            Fonts.Add(data);
            ++AssetsCount;
        }
    }
}
