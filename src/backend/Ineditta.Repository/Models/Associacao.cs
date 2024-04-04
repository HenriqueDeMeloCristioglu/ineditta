using Ineditta.Application.Sindicatos.Laborais.Entities;
using Ineditta.Application.Sindicatos.Patronais.Entities;

namespace Ineditta.Repository.Models;

public partial class Associacao
{
    public int IdAssociacao { get; set; }

    public string Sigla { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Grau { get; set; } = null!;

    public string Nome { get; set; } = null!;

    public string AreaGeoeconomica { get; set; } = null!;

    public string Grupo { get; set; } = null!;

    public string Classe { get; set; } = null!;

    public string Categoria { get; set; } = null!;

    public string Logradouro { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Numero { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public string? Estado { get; set; }

    public string Municipio { get; set; } = null!;

    public string? Email { get; set; }

    public string? Email2 { get; set; }

    public string? Email3 { get; set; }

    public string Telefone { get; set; } = null!;

    public string? Ramal { get; set; }

    public string? Website { get; set; }

    public string? Twitter { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public string? Ddd { get; set; }

    public virtual ICollection<SindicatoLaboral> SindEmpConfederacaoIdAssociacaoNavigations { get; set; } = new List<SindicatoLaboral>();

    public virtual ICollection<SindicatoLaboral> SindEmpFederacaoIdAssociacaoNavigations { get; set; } = new List<SindicatoLaboral>();

    public virtual ICollection<SindicatoPatronal> SindPatrConfederacaoIdAssociacaoNavigations { get; set; } = new List<SindicatoPatronal>();

    public virtual ICollection<SindicatoPatronal> SindPatrFederacaoIdAssociacaoNavigations { get; set; } = new List<SindicatoPatronal>();
}
