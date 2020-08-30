namespace BLIT64_Common.IO
{
    public class ImageResource
    {
        public int Width { get; }
        public int Height { get; }
        public byte[] ImageData { get; }

        internal ImageResource(byte[] image_data, int width, int height)
        {
            ImageData = image_data;
            Width = width;
            Height = height;
        }
    }
}
