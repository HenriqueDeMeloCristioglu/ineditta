namespace Ineditta.API.ViewModels.Sindicatos
{
    public class MandatosSindicaisRequest
    {
        public IEnumerable<int> SindeIds { get; set; } = new List<int>();
        public IEnumerable<int> SindpIds { get; set; } = new List<int>();
        public int GrupoEconomicoId { get; set; }
    }
}
