using CSharpFunctionalExtensions;

namespace Ineditta.BuildingBlocks.Core.Renderers.Html
{
    public interface IHtmlRenderer
    {
        ValueTask<Result<string>> RenderAsync(string templateKey, string content, object? model = null);
    }
}
