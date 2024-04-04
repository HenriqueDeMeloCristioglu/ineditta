namespace Ineditta.BuildingBlocks.Core.FileStorage
{
    public record FileDto(string Name, byte[] Content, string Folder)
    {
    }
}