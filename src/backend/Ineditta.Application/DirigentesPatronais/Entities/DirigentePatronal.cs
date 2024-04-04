using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CSharpFunctionalExtensions;

namespace Ineditta.Application.DirigentesPatronais.Entities
{
    public class DirigentePatronal : Entity
    {
        private DirigentePatronal(string nome, string funcao, string? situacao, DateOnly dataInicioMandato, DateOnly dataFimMandato, int sindicatoPatronalId, int? estabelecimentoId)
        {
            Nome = nome;
            Funcao = funcao;
            Situacao = situacao;
            DataInicioMandato = dataInicioMandato;
            DataFimMandato = dataFimMandato;
            SindicatoPatronalId = sindicatoPatronalId;
            EstabelecimentoId = estabelecimentoId;
        }

        protected DirigentePatronal() { }

        public string Nome { get; private set; } = null!;
        public string Funcao { get; private set; } = null!;
        public string? Situacao { get; private set; }
        public DateOnly DataInicioMandato { get; private set; }
        public DateOnly DataFimMandato { get; private set; }
        public int SindicatoPatronalId { get; private set; }
        public int? EstabelecimentoId { get; private set; }

        public static Result<DirigentePatronal> Criar(string nome, string funcao, string? situacao, DateOnly dataInicioMandato, DateOnly dataFimMandato, int sindicatoPatronalId, int? estabelecimentoId)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<DirigentePatronal>("Você deve fornecer o nome do dirigente");
            if (string.IsNullOrEmpty(funcao)) return Result.Failure<DirigentePatronal>("Você deve fornecer a função do dirente");
            if (dataFimMandato < dataInicioMandato) return Result.Failure<DirigentePatronal>("A data final não pode ser anterior a data inicial");
            if (sindicatoPatronalId <= 0) return Result.Failure<DirigentePatronal>("O id do sindicato patronal deve ser maior que 0");
            if (estabelecimentoId <= 0) return Result.Failure<DirigentePatronal>("O id do estabelecimento deve ser maior que 0");

            var dirigentePatronal = new DirigentePatronal(
                nome.ToUpperInvariant(),
                funcao,
                situacao,
                dataInicioMandato,
                dataFimMandato,
                sindicatoPatronalId,
                estabelecimentoId
            );

            return Result.Success(dirigentePatronal);
        }

        public Result Atualizar(string nome, string funcao, string? situacao, DateOnly dataInicioMandato, DateOnly dataFimMandato, int sindicatoPatronalId, int? estabelecimentoId)
        {
            if (string.IsNullOrEmpty(nome)) return Result.Failure<DirigentePatronal>("Você deve fornecer o nome do dirigente");
            if (string.IsNullOrEmpty(funcao)) return Result.Failure<DirigentePatronal>("Você deve fornecer a função do dirente");
            if (dataFimMandato < dataInicioMandato) return Result.Failure<DirigentePatronal>("A data final não pode ser anterior a data inicial");
            if (sindicatoPatronalId <= 0) return Result.Failure<DirigentePatronal>("O id do sindicato patronal deve ser maior que 0");
            if (estabelecimentoId <= 0) return Result.Failure<DirigentePatronal>("O id do estabelecimento deve ser maior que 0");

            Nome = nome.ToUpperInvariant();
            Funcao = funcao;
            Situacao = situacao;
            DataInicioMandato = dataInicioMandato;
            DataFimMandato = dataFimMandato;
            SindicatoPatronalId = sindicatoPatronalId;
            EstabelecimentoId = estabelecimentoId;

            return Result.Success();
        }
    }
}
