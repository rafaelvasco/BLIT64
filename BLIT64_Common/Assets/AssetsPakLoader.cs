using System.IO;
using Binaron.Serializer;

namespace BLIT64_Common
{
    public static class AssetsPakLoader
    {
        public static AssetsPak LoadPak(Stream stream)
        {
            var pak = BinaronConvert.Deserialize<AssetsPak>(stream);

            return pak;
        }
    }
}
