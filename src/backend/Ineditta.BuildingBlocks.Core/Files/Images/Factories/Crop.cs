using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Ineditta.BuildingBlocks.Core.Files.Images.Factories
{
    public static class Crop
    {
        public static MemoryStream VerticalWithPercent(MemoryStream stream, decimal height)
        {
            stream.Seek(0, SeekOrigin.Begin);

            using var image = Image.Load(stream);

            decimal newHeight = image.Height * height;
            decimal topOffset = image.Height * height;
            decimal bottomOffset = image.Height - newHeight;

            if (topOffset < image.Height)
            {
                image.Mutate(x => x.Crop(new Rectangle(0, (int)topOffset, image.Width, Math.Min((int)newHeight, image.Height - (int)topOffset))));
            }

            if (bottomOffset > 0)
            {
                image.Mutate(x => x.Crop(new Rectangle(0, 0, image.Width, Math.Min((int)newHeight, (int)bottomOffset))));
            }

            var resizedStream = new MemoryStream();

            image.Save(resizedStream, new JpegEncoder());

            return resizedStream;
        }
    }
}
