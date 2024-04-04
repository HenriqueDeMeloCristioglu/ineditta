namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class Abrangencia
    {
        public Abrangencia(int? id, string? uf, string? municipio)
        {
            Id = id;
            Uf = uf;
            Municipio = municipio;
        }

        public int? Id { get; private set; }
        public string? Uf { get; private set; }
        public string? Municipio { get; private set; }
    }
}
