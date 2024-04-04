namespace Ineditta.Repository.Models;

public partial class TemporarioClausulageral
{
    public int IdtemporarioClausulageral { get; set; }

    public string? TexClau { get; set; }

    public int? DocSindIdDocumento { get; set; }

    public int EstruturaIdEstruturaclausula { get; set; }
}
