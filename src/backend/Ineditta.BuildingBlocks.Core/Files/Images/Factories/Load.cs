using Ineditta.BuildingBlocks.Core.Files.Images.Dtos;

using SixLabors.ImageSharp;

namespace Ineditta.BuildingBlocks.Core.Files.Images.Factories
{
    public static class Load
    {
        public static ImageLoadedDto FromMemoryStream(MemoryStream image)
        {
            using var loadedImage = Image.Load(image);

            return new ImageLoadedDto
            {
                Width = loadedImage.Width,
                Height = loadedImage.Height
            };
        }
    }
}
