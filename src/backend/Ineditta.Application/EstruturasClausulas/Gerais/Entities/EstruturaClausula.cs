using CSharpFunctionalExtensions;

using Ineditta.Application.EstruturasClausulas.GruposEconomicos.Entities;

namespace Ineditta.Application.EstruturasClausulas.Gerais.Entities
{
    public class EstruturaClausula : Entity<int>
    {
        private readonly List<EstruturaClausulaGrupoEconomico> _gruposEconomicos;

        protected EstruturaClausula()
        {
            _gruposEconomicos = new List<EstruturaClausulaGrupoEconomico>();
        }

        public EstruturaClausula(int grupoClausulaId, string nome, Tipo tipo, bool classe, bool calendario, bool tarefa, bool resumivel, List<EstruturaClausulaGrupoEconomico>? gruposEconomicos = null, string? instrucaoIa = null, int? maximoPalavrasIA = null)
        {
            GrupoClausulaId = grupoClausulaId;
            Nome = nome;
            Tipo = tipo;
            Classe = classe;
            Calendario = calendario;
            Tarefa = tarefa;
            Resumivel = resumivel;

            _gruposEconomicos = gruposEconomicos ?? new List<EstruturaClausulaGrupoEconomico>();
            InstrucaoIa = instrucaoIa;
            MaximoPalavrasIA = maximoPalavrasIA;
        }

        public int GrupoClausulaId { get; private set; }
        public string Nome { get; private set; } = null!;
        public Tipo Tipo { get; private set; }
        public bool Classe { get; private set; }
        public bool Calendario { get; private set; }
        public bool Tarefa { get; private set; }
        public bool Resumivel { get; private set; }
        public string? InstrucaoIa { get; private set; }
        public int? MaximoPalavrasIA { get; private set; }
        public IEnumerable<EstruturaClausulaGrupoEconomico> GruposEconomicos => _gruposEconomicos.AsReadOnly();
    }
}
