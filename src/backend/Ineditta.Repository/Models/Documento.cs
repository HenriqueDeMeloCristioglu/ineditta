using System;
using System.Collections.Generic;

using Ineditta.Application.TiposDocumentos.Entities;

namespace Ineditta.Repository.Models;

public partial class Documento
{
    public int Iddocumentos { get; set; }

    public string? DescricaoDocumento { get; set; }

    public string? OrigemDocumento { get; set; }

    public int TipoDocIdtipoDoc { get; set; }

    public int? SindEmpIdSinde { get; set; }

    public int? SindPatrIdSindp { get; set; }

    public string? NumeroLei { get; set; }

    public DateOnly? VigenciaInicial { get; set; }

    public DateOnly? VigenciaFinal { get; set; }

    public string? CaminhoArquivo { get; set; }

    public string? DocumentoRestrito { get; set; }

    public string? Anuencia { get; set; }

    public string? Status { get; set; }

    public string? ComentarioLegislacao { get; set; }

    public string? FonteWeb { get; set; }

    public int? UsuarioAdmIdUser { get; set; }

    public DateTime DataUpload { get; set; }

    public virtual ICollection<DadosSdf> DadosSdfs { get; set; } = new List<DadosSdf>();

    public virtual ICollection<DocumentoAssunto> DocumentoAssuntos { get; set; } = new List<DocumentoAssunto>();

    public virtual ICollection<DocumentosAbrangencium> DocumentosAbrangencia { get; set; } = new List<DocumentosAbrangencium>();

    public virtual TipoDocumento TipoDocIdtipoDocNavigation { get; set; } = null!;
}
