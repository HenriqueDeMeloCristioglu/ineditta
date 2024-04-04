using Ineditta.Application.Documentos.Sindicais.Entities;

namespace Ineditta.Repository.Models;

public partial class AnuenciaUsuario
{
    public int IdanuenciaUsuarios { get; set; }

    public int DocumentosIddocumentos { get; set; }

    public int UsuarioAdmIdUser { get; set; }

    public DateTime DatadocAnuencia { get; set; }

    public virtual DocumentoSindical DocumentosIddocumentosNavigation { get; set; } = null!;
}
