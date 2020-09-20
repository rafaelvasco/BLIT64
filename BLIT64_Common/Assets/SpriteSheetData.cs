using System.Collections.Generic;

namespace BLIT64_Common
{
    public class SpriteSheetData
    {
        public string Id;
        public byte[] ImageData;
        public int ImageWidth;
        public int ImageHeight;
        public int CellSize;
        public Dictionary<string, int> SpriteMap;
    }
}
