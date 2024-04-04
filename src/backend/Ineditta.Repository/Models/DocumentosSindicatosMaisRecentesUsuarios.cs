namespace Ineditta.Repository.Models
{
    public partial class DocumentosSindicatosMaisRecentesUsuarios
    {
        public int SindicatoLaboralId { get; set; }
        public int AnoMesValidadeInicial { get; set; }
        public int AnoMesValidadeFinal { get; set; }
        public int DocumentoSindicalId { get; set; }
        public int RowNum { get; set; }
        public int UsuarioId { get; set; }
        public int SindicatoPatronalId { get; set; }
        public long Id { get; set; }
    }
}
