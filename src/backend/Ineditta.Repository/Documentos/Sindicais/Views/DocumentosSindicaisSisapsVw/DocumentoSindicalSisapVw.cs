using Ineditta.Application.Documentos.Sindicais.Dtos;

using Cnae = Ineditta.Application.Documentos.Sindicais.Dtos.Cnae;

namespace Ineditta.Repository.Documentos.Sindicais.Views.DocumentosSindicaisSisapsVw
{
    public class DocumentoSindicalSisapVw
    {
        public int Id { get; set; }
        public string NomeDoc { get; set; }
        public IEnumerable<Cnae>? Cnaes { get; set; }
        public DateOnly? DataAprovacao { get; set; }
        public DateOnly? DataValidadeInicial { get; set; }
        public DateOnly? DataValidadeFinal { get; set; }
        public DateOnly? DataSla { get; set; }
        public IEnumerable<SindicatoLaboral>? SindicatosLaborais { get; set; }
        public IEnumerable<SindicatoPatronal>? SindicatosPatronais { get; set; }
        public string? UsuarioAprovador { get; set; }
        public DateOnly? DataAssinatura { get; set; }
        public IEnumerable<int>? CnaeSubclasseCodigos { get; set; }

    }
}
