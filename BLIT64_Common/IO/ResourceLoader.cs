using System.IO;
using StbImageSharp;

namespace BLIT64_Common.IO
{
    public static class ResourceLoader
    {
        public static ImageResource LoadImage(string path)
        {
            using var stream = File.OpenRead(path);

            var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            return new ImageResource(image.Data, image.Width, image.Height);
        }

    }
}
