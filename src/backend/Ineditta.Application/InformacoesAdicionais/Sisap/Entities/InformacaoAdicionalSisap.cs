using CSharpFunctionalExtensions;

namespace Ineditta.Application.InformacoesAdicionais.Sisap.Entities
{
    public class InformacaoAdicionalSisap : Entity<int>
    {
        protected InformacaoAdicionalSisap() { }
        private InformacaoAdicionalSisap(int documentoSindicalId, int clausulaGeralId, int estruturaClausulaId, int nomeInformacaoEstruturaClausulaId, int tipoinformacaoadicionalId, int inforamcacaoAdicionalGrupoId, int sequenciaItem, int sequenciaLinha, string? texto, decimal? numerico, string? descricao, string? data, decimal? percentual, string? hora, string? combo)
        {
            DocumentoSindicalId = documentoSindicalId;
            ClausulaGeralId = clausulaGeralId;
            EstruturaClausulaId = estruturaClausulaId;
            NomeInformacaoEstruturaClausulaId = nomeInformacaoEstruturaClausulaId;
            TipoinformacaoadicionalId = tipoinformacaoadicionalId;
            InforamcacaoAdicionalGrupoId = inforamcacaoAdicionalGrupoId;
            SequenciaItem = sequenciaItem;
            SequenciaLinha = sequenciaLinha;
            Texto = texto;
            Numerico = numerico;
            Descricao = descricao;
            Data = data;
            Percentual = percentual;
            Hora = hora;
            Combo = combo;
        }

        public int DocumentoSindicalId { get; private set; }
        public int ClausulaGeralId { get; private set; }
        public int EstruturaClausulaId { get; private set; }
        public int NomeInformacaoEstruturaClausulaId { get; private set; }
        public int TipoinformacaoadicionalId { get; private set; }
        public int InforamcacaoAdicionalGrupoId { get; private set; }
        public int SequenciaItem { get; private set; }
        public int SequenciaLinha { get; private set; }
        public string? Texto { get; private set; }
        public decimal? Numerico { get; private set; }
        public string? Descricao { get; private set; }
        public string? Data { get; private set; }
        public decimal? Percentual { get; private set; }
        public string? Hora { get; private set; }
        public string? Combo { get; private set; }

        public static Result<InformacaoAdicionalSisap> Criar(int documentoSindicalId, int clausulaGeralId, int estruturaClausulaId, int nomeInformacaoEstruturaClausulaId, int tipoinformacaoadicionalId, int inforamcacaoAdicionalGrupoId, int sequenciaItem, int sequenciaLinha, string? texto, decimal? numerico, string? descricao, string? data, decimal? percentual, string? hora, string? combo)
        {
            if (documentoSindicalId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Documento não pode ser nulo");
            }

            if (clausulaGeralId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id da Clausulas não pode ser nulo");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id da Estrutura Clausula não pode ser nulo");
            }

            if (tipoinformacaoadicionalId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Tipo de Informação Adicional não pode ser nulo");
            }

            if (inforamcacaoAdicionalGrupoId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Grupo de Informação Adicional não pode ser nulo");
            }

            if (nomeInformacaoEstruturaClausulaId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do do Nome da Informação Adicional não pode ser nulo");
            }

            if (sequenciaItem <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Sequência do Item não pode ser nulo");
            }

            if (sequenciaLinha <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Sequência da Linha não pode ser nulo");
            }

            var informacaoAdicionalSisap = new InformacaoAdicionalSisap(documentoSindicalId, clausulaGeralId, estruturaClausulaId, nomeInformacaoEstruturaClausulaId, tipoinformacaoadicionalId, inforamcacaoAdicionalGrupoId, sequenciaItem, sequenciaLinha, texto, numerico, descricao, data, percentual, hora, combo);

            return Result.Success(informacaoAdicionalSisap);
        }

        public Result Atualizar(int documentoSindicalId, int clausulaGeralId, int estruturaClausulaId, int nomeInformacaoEstruturaClausulaId, int tipoinformacaoadicionalId, int inforamcacaoAdicionalGrupoId, int sequenciaItem, int sequenciaLinha, string? texto, decimal? numerico, string? descricao, string? data, decimal? percentual, string? hora, string? combo)
        {
            if (documentoSindicalId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Documento não pode ser nulo");
            }

            if (clausulaGeralId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id da Clausulas não pode ser nulo");
            }

            if (estruturaClausulaId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id da Estrutura Clausula não pode ser nulo");
            }

            if (tipoinformacaoadicionalId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Tipo de Informação Adicional não pode ser nulo");
            }

            if (inforamcacaoAdicionalGrupoId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do Grupo de Informação Adicional não pode ser nulo");
            }

            if (nomeInformacaoEstruturaClausulaId <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Id do do Nome da Informação Adicional não pode ser nulo");
            }

            if (sequenciaItem <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Sequência do Item não pode ser nulo");
            }

            if (sequenciaLinha <= 0)
            {
                return Result.Failure<InformacaoAdicionalSisap>("Sequência da Linha não pode ser nulo");
            }

            DocumentoSindicalId = documentoSindicalId;
            ClausulaGeralId = clausulaGeralId;
            EstruturaClausulaId = estruturaClausulaId;
            NomeInformacaoEstruturaClausulaId = nomeInformacaoEstruturaClausulaId;
            TipoinformacaoadicionalId = tipoinformacaoadicionalId;
            InforamcacaoAdicionalGrupoId = inforamcacaoAdicionalGrupoId;
            SequenciaItem = sequenciaItem;
            SequenciaLinha = sequenciaLinha;
            Texto = texto;
            Numerico = numerico;
            Descricao = descricao;
            Data = data;
            Percentual = percentual;
            Hora = hora;
            Combo = combo;

            return Result.Success();
        }
    }
}
