namespace Ineditta.Application.Documentos.Sindicais.Dtos
{
    public class Cnae
    {
        public Cnae(int id, string? subclasse)
        {
            Id = id;
            Subclasse = subclasse;
        }

        public int Id { get; private set; }
        public string? Subclasse { get; private set; }
    }
}
