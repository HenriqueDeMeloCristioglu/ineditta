namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class SindicatoLaboral
    {
        public SindicatoLaboral()
        {

        }
        public SindicatoLaboral(int? id, string? uf, string? cnpj, string? sigla, string? municipio, string? codigo)
        {
            Id = id;
            Uf = uf;
            Cnpj = cnpj;
            Sigla = sigla;
            Municipio = municipio;
            Codigo = codigo;
        }

        public int? Id { get; set; }
        public string? Uf { get; set; }
        public string? Cnpj { get; set; }
        public string? Sigla { get; set; }
        public string? Municipio { get; set; }
        public string? Codigo { get; set; }
        public string? Denominacao { get; set; }
    }
}
