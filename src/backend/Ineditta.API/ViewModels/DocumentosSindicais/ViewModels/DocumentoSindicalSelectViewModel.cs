namespace Ineditta.API.ViewModels.DocumentosSindicais.ViewModels
{
    public class DocumentoSindicalSelectViewModel
    {
        public int? Id { get; set; }
        public string? Description { get; set; }
        public DateOnly? ValidadeInicial { get; set; }
        public DateOnly? ValidadeFinal { get; set; }    
    }
}
