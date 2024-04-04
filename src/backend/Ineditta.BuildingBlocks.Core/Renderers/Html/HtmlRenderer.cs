using System.Reflection;

using CSharpFunctionalExtensions;

using Microsoft.Extensions.Logging;

using RazorLight;

namespace Ineditta.BuildingBlocks.Core.Renderers.Html
{
    public class HtmlRenderer : IHtmlRenderer
    {
        private readonly static RazorLightEngine Engine = new RazorLightEngineBuilder()
                           .UseMemoryCachingProvider()
                           .EnableDebugMode()
                           .Build();
        
        private readonly ILogger<HtmlRenderer> _logger;
        public HtmlRenderer(ILogger<HtmlRenderer> logger)
        {
            _logger = logger;
        }

        public async ValueTask<Result<string>> RenderAsync(string templateKey, string content, object? model = null)
        {
            try
            {
                var cacheResult = Engine.Handler.Cache.RetrieveTemplate(templateKey);
                var result = string.Empty;

                if (cacheResult.Success)
                {
                    var templatePage = cacheResult.Template.TemplatePageFactory();
                    result = await Engine.RenderTemplateAsync(templatePage, model);
                }
                else
                {
                    result = await Engine.CompileRenderStringAsync(templateKey, content, model);
                }

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renderizar html", model);

                return Result.Failure<string>("Não foi possível fazer o bind do template html");
            }
        }
    }
}
