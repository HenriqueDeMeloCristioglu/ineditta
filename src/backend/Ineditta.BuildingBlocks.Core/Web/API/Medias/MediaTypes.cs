using Microsoft.AspNetCore.StaticFiles;

namespace Ineditta.BuildingBlocks.Core.Web.API.Medias
{
    public static class MediaTypes
    {
        private readonly static FileExtensionContentTypeProvider Provider = new();

        public readonly static string Excel = Provider.Mappings[".xlsx"];
    }
}
