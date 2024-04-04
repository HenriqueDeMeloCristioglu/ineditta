using Ineditta.API.ViewModels.Clausulas.ViewModels;

namespace Ineditta.API.ViewModels.Clausulas.Requests
{
    public class PdfClausulaRequest
    {
        public IEnumerable<ClausulaViewModel>? Clausulas { get; set; }
    }
}
