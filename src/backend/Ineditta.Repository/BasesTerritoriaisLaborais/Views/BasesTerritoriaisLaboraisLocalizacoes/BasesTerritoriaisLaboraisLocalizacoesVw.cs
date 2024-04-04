namespace Ineditta.Repository.BasesTerritoriaisLaborais.Views.BasesTerritoriaisLaboraisLocalizacoes
{
    public class BasesTerritoriaisLaboraisLocalizacoesVw
    {
        public int Id { get; set; }
        public string Sigla { get; set; }
        public string Uf { get; set; }
        public string Municipio { get; set; }
        public string Pais { get; set; }
        public int SindicatoLaboralId { get; set; }
    }
}
