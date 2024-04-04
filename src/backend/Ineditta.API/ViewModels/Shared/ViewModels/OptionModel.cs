#pragma warning disable CA1716 // Identifiers should not match keywords
namespace Ineditta.API.ViewModels.Shared.ViewModels
{
    public class OptionModel<TId>
    {
        public TId? Id { get; set; }
        public string? Description { get; set; }
    }
}
#pragma warning restore CA1716 // Identifiers should not match keywords