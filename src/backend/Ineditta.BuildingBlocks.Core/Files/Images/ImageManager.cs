using Ineditta.BuildingBlocks.Core.Files.Images.Dtos;
using Ineditta.BuildingBlocks.Core.Files.Images.Factories;

namespace Ineditta.BuildingBlocks.Core.Files.Images
{
    public static class ImageManager
    {
        public static Stream? Crop(MemoryStream url, decimal percentToCrop)
        {
            if (url == null)
            {
                return default;
            }

            url = Factories.Crop.VerticalWithPercent(url, percentToCrop);

            return url;
        }

        public static Stream? Crop(MemoryStream url, decimal percentToCrop, Func<ImageLoadedDto, bool> ruleCheckIfCrop)
        {
            if (url == null)
            {
                return default;
            }

            var dto = Load.FromMemoryStream(url);

            if (ruleCheckIfCrop(dto))
            {
                url = Factories.Crop.VerticalWithPercent(url, percentToCrop);
            }

            return url;
        }
    }
}
