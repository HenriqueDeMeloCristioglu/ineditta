using CSharpFunctionalExtensions;

using Ineditta.Application.Usuarios.Entities;
using Ineditta.BuildingBlocks.Core.Domain.Contracts;

namespace Ineditta.Application.InformacoesAdicionais.Cliente.Entities
{
    public class InformacaoAdicionalCliente : Entity, IAuditable
    {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected InformacaoAdicionalCliente() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private InformacaoAdicionalCliente(int grupoEconomicoId, IEnumerable<InformacaoAdicional>? informacoes, int documentoSindicalId, IEnumerable<ObservacaoAdicional>? observacoesAdicionais, string? orientacao, string? outrasInformacoes)
        {
            GrupoEconomicoId = grupoEconomicoId;
            _informacoesAdicionais = informacoes?.ToList() ?? new List<InformacaoAdicional>();
            _observacoesAdicionais = observacoesAdicionais?.ToList() ?? new List<ObservacaoAdicional>();
            DocumentoSindicalId = documentoSindicalId;
            Aprovado = false;
            Orientacao = orientacao;
            OutrasInformacoes = outrasInformacoes;
        }

        public int GrupoEconomicoId { get; private set; }
        public int DocumentoSindicalId { get; private set; }


        private readonly List<ObservacaoAdicional> _observacoesAdicionais;
        public IEnumerable<ObservacaoAdicional>? ObservacoesAdicionais => _observacoesAdicionais?.AsReadOnly();

        private readonly List<InformacaoAdicional> _informacoesAdicionais;
        public IEnumerable<InformacaoAdicional>? InformacoesAdicionais => _informacoesAdicionais?.AsReadOnly();
        public bool Aprovado { get; private set; }
        public string? Orientacao { get; private set; }
        public string? OutrasInformacoes { get; private set; }

        internal static Result<InformacaoAdicionalCliente> Criar(Usuario usuario, int documentoSindicalId, IEnumerable<InformacaoAdicional>? informacoesAdicionais, IEnumerable<ObservacaoAdicional>? observacoesAdicionais, string? orientacao, string? outrasInformacoes)
        {
            if (usuario is null)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Informe o usuário");
            }

            if (usuario.Nivel == Nivel.Ineditta)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não pode ser Ineditta");
            }

            if (!usuario.TemGrupoEconomico())
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não tem grupo econômico");
            }

            if (documentoSindicalId <= 0)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Informe o documento sindical");
            }

            var informacaoAdicionalCliente = new InformacaoAdicionalCliente(usuario.GrupoEconomicoId!.Value, informacoesAdicionais, documentoSindicalId, observacoesAdicionais, orientacao, outrasInformacoes);

            return Result.Success(informacaoAdicionalCliente);
        }


        internal Result Atualizar(Usuario usuario, IEnumerable<InformacaoAdicional>? informacoesAdicionais, IEnumerable<ObservacaoAdicional>? observacoesAdicionais, string? orientacao, string? outrasInformacoes)
        {
            if (usuario is null)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Informe o usuário");
            }

            if (usuario.Nivel == Nivel.Ineditta)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não pode ser Ineditta");
            }

            if (!usuario.TemGrupoEconomico())
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não tem grupo econômico");
            }

            if (usuario.GrupoEconomicoId != GrupoEconomicoId)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Grupo econômico do usuário de edição deve ser o mesmo que o cadastrado para essas informações adicionais");
            }

            if (informacoesAdicionais is null)
            {
                return Result.Failure("Infrome o grupo econômico");
            }
            

            foreach (var info in informacoesAdicionais)
            {
                var informacaoAdicionalExistentes = _informacoesAdicionais.Find(e => e.ClausulaGeralEstruturaId == info.ClausulaGeralEstruturaId);

                if (informacaoAdicionalExistentes != null)
                {
                    if (info.Valor.Length <= 0)
                    {
                        _informacoesAdicionais.Remove(informacaoAdicionalExistentes);
                    } else
                    {
                        informacaoAdicionalExistentes.Atualizar(info.ClausulaGeralEstruturaId, info.Valor);
                    }
                }
                else
                {
                    var novaInformacaoAdicional = InformacaoAdicional.Criar(info.ClausulaGeralEstruturaId, info.Valor);

                    _informacoesAdicionais.Add(novaInformacaoAdicional.Value);
                }
            }


            if (observacoesAdicionais is not null)
            {
                foreach (var obsr in observacoesAdicionais)
                {
                    var observacaoAdicionalExistente = _observacoesAdicionais.Find(e => e.ClausulaId == obsr.ClausulaId && obsr.Tipo == e.Tipo);

                    if (observacaoAdicionalExistente != null)
                    {
                        if (obsr.Valor.Length <= 0)
                        {
                            _observacoesAdicionais.Remove(observacaoAdicionalExistente);
                        } else
                        {
                            observacaoAdicionalExistente.Atualizar(obsr.Valor);
                        }
                    }
                    else
                    {
                        var novaInformacaoAdicional = ObservacaoAdicional.Criar(obsr.ClausulaId, obsr.Valor, obsr.Tipo);

                        _observacoesAdicionais.Add(novaInformacaoAdicional.Value);
                    }
                }
            }

            Orientacao = orientacao;
            OutrasInformacoes = outrasInformacoes;

            return Result.Success();
        }

        internal Result Aprovar(Usuario usuario)
        {
            if (usuario is null)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Informe o usuário");
            }

            if (usuario.Nivel == Nivel.Ineditta)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não pode ser Ineditta");
            }

            if (!usuario.TemGrupoEconomico())
            {
                return Result.Failure<InformacaoAdicionalCliente>("Usuário não tem grupo econômico");
            }

            if (usuario.GrupoEconomicoId != GrupoEconomicoId)
            {
                return Result.Failure<InformacaoAdicionalCliente>("Grupo econômico do usuário de edição deve ser o mesmo que o cadastrado para essas informações adicionais");
            }

            Aprovado = true;

            return Result.Success();
        }
    }
}
