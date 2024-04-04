using Ineditta.Application.Sindicatos.Laborais.Entities;

namespace Ineditta.Repository.Models;

public partial class CentralSindical
{
    public int IdCentralsindical { get; set; }

    public string Sigla { get; set; } = null!;

    public string Cnpj { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string NomeCentralsindical { get; set; } = null!;

    public string Logradouro { get; set; } = null!;

    public string? Complemento { get; set; }

    public string Numero { get; set; } = null!;

    public string Bairro { get; set; } = null!;

    public string Cep { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Municipio { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Email2 { get; set; }

    public string? Email3 { get; set; }

    public string? Ddd { get; set; }

    public string Telefone { get; set; } = null!;

    public string? Website { get; set; }

    public string? Twitter { get; set; }

    public string? Facebook { get; set; }

    public string? Instagram { get; set; }

    public virtual ICollection<SindicatoLaboral> SindEmps { get; set; } = new List<SindicatoLaboral>();
}
