namespace Ineditta.API.ViewModels.Usuariosadm.ViewModels
{
    public class UsuariosAdmViewModel
    {
        public long Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Cargo { get; set; }
        public string? Telefone { get; set; }
        public string? Ramal { get; set; }
        public string? Departamento { get; set; }
        public int? IdSuperior { get; set; }
        public DateTime? DataCriacao { get; set; }
        public string? NomeUserCriador { get; set; }
    }
}
