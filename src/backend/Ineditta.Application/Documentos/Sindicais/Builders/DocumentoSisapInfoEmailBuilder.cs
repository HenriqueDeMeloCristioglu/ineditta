using Ineditta.Application.Clausulas.Gerais.Dtos;
using Ineditta.Application.ClientesUnidades.Entities;
using Ineditta.Application.Cnaes.Entities;
using Ineditta.Application.Documentos.Sindicais.Entities;
using Ineditta.Application.Documentos.Sindicais.Dtos;
using Ineditta.Application.Localizacoes.Repositories;
using Ineditta.Application.Sindicatos.Patronais.Entities;
using Ineditta.Application.TiposDocumentos.Entities;

using Cnae = Ineditta.Application.Cnaes.Entities.Cnae;
using SindicatoLaboral = Ineditta.Application.Sindicatos.Laborais.Entities.SindicatoLaboral;
using SindicatoPatronal = Ineditta.Application.Sindicatos.Patronais.Entities.SindicatoPatronal;

namespace Ineditta.Application.Documentos.Sindicais.Builders
{
    public class DocumentoSisapInfoEmailBuilder
    {
        private readonly ILocalizacaoRepository _localizacaoRepository;
        public DocumentoAprovadoEmailDto InfoEmail { get; set; }

        public DocumentoSisapInfoEmailBuilder(ILocalizacaoRepository localizacaoRepository)
        {
            InfoEmail = new DocumentoAprovadoEmailDto();
            _localizacaoRepository = localizacaoRepository;
        }

        public void AdicionarAbrangencia(DocumentoSindical documento)
        {
            InfoEmail.Abrangencia = documento.Abrangencias is not null ?
                string.Join(", ", documento.Abrangencias.Select(a => a.Municipio + "/" + a.Uf).ToList()) : "";
        }

        public void AdicionarSindicatosLaborais(IEnumerable<SindicatoLaboral>? sindicatosLaboraisDoUsuario, DocumentoSindical documento)
        {
            if (sindicatosLaboraisDoUsuario is null) return;

            var sindicatosLaboraisDoUsuarioNoDocumento = sindicatosLaboraisDoUsuario.Where(
                slu => documento.SindicatosLaborais is not null && documento.SindicatosLaborais.Any(sld => sld.Id == slu.Id)
            );

            if (sindicatosLaboraisDoUsuarioNoDocumento is null || !sindicatosLaboraisDoUsuarioNoDocumento.Any()) return;

            var sindicatosLaboraisDoUsuarioNoDocumentoFormatado = sindicatosLaboraisDoUsuarioNoDocumento
                                                                    .Select(sl => sl.Sigla + " / " + sl.Denominacao)
                                                                    .ToList();

            var sindicatosLaborais = string.Join("; ", sindicatosLaboraisDoUsuarioNoDocumentoFormatado);

            InfoEmail.SindicatosLaborais = sindicatosLaborais;
        }

        public void AdicionarSindicatosPatronais(IEnumerable<SindicatoPatronal>? sindicatosPatronaisDoUsuario, DocumentoSindical documento)
        {
            if (sindicatosPatronaisDoUsuario is null) return;

            var sindicatosPatronaisDoUsuarioNoDocumento = sindicatosPatronaisDoUsuario.Where(
                spu => documento.SindicatosPatronais is not null && documento.SindicatosPatronais.Any(spd => spd.Id == spu.Id)
            );

            if (sindicatosPatronaisDoUsuarioNoDocumento is null || !sindicatosPatronaisDoUsuarioNoDocumento.Any()) return;

            var sindicatosPatronaisDoUsuarioNoDocumentoFormatado = sindicatosPatronaisDoUsuarioNoDocumento
                                                                    .Select(sl => sl.Sigla + " / " + sl.Denominacao)
                                                                    .ToList();

            var sindicatosPatronais = string.Join("; ", sindicatosPatronaisDoUsuarioNoDocumentoFormatado);

            InfoEmail.SindicatosPatronais = sindicatosPatronais;
        }

        public async Task AdicionarUnidadesCliente(IEnumerable<ClienteUnidade>? clientesUnidadesUsuarioDocumento, bool adicionarLocalidades)
        {
            if (clientesUnidadesUsuarioDocumento is null || !clientesUnidadesUsuarioDocumento.Any()) return;
            InfoEmail.Estabelecimentos = clientesUnidadesUsuarioDocumento;

            if (adicionarLocalidades)
            {
                var localizacoesPorClienteUnidade = new Dictionary<int, string>();

                foreach (var unidade in clientesUnidadesUsuarioDocumento)
                {
                    var localizacao = await _localizacaoRepository.ObterPorIdAsync(unidade.LocalizacaoId);
                    var localizacaoString = localizacao is not null ? localizacao.Municipio + "-" + localizacao.Uf.Id : "";
                    localizacoesPorClienteUnidade.Add(unidade.Id, localizacaoString);
                }

                InfoEmail.LocalizacoesUnidades = localizacoesPorClienteUnidade;
            }
        }

        public void AdicionarAtividadesEconomicas(IEnumerable<Cnae>? atividadesEconomicasUnidades, DocumentoSindical documento)
        {
            var atividadesEconomicasUnidadesIds = atividadesEconomicasUnidades is null ? new List<int>() : atividadesEconomicasUnidades.Select(a => a.Id);

            var atividadesEconomicas = documento.Cnaes is not null ?
                string.Join("; ", documento.Cnaes.Where(aedc => atividadesEconomicasUnidadesIds.Any(aeuids => aedc.Id == aeuids)).Select(cc => cc.Subclasse).ToList()) :
                "";

            InfoEmail.AtividadesEconomicas = atividadesEconomicas;
        }

        public void AdicionarVigencias(DocumentoSindical documento)
        {
            InfoEmail.VigenciaInicial = documento.DataValidadeInicial;
            InfoEmail.VigenciaFinal = documento.DataValidadeFinal;
        }

        public void AdicionarNomeDocumento(TipoDocumento tipoDocumento)
        {
            InfoEmail.NomeDocumento = tipoDocumento is not null ? tipoDocumento.Nome : "";
        }

        public void AdicionarSla(DocumentoSindical documento)
        {
            InfoEmail.DataSla = documento.DataSla;
        }

        public void AdicionarAssunto(TipoDocumento tipoDocumento, DocumentoSindical documento, IEnumerable<SindicatoLaboral>? sindicatosLaboraisDoUsuario, IEnumerable<SindicatoPatronal>? sindicatosPatronaisDoUsuario)
        {
            var sindicatosLaboraisDoUsuarioNoDocumento = sindicatosLaboraisDoUsuario is not null ? sindicatosLaboraisDoUsuario.Where(
                slu => documento.SindicatosLaborais is not null && documento.SindicatosLaborais.Any(sld => sld.Id == slu.Id)
            ) : null;
            var primeiroSindicatoLaboralUsuario = sindicatosLaboraisDoUsuarioNoDocumento is not null ? sindicatosLaboraisDoUsuarioNoDocumento.FirstOrDefault() : null;

            var sindicatosPatronaisDoUsuarioNoDocumento = sindicatosPatronaisDoUsuario is not null ? sindicatosPatronaisDoUsuario.Where(
                spu => documento.SindicatosPatronais is not null && documento.SindicatosPatronais.Any(spd => spd.Id == spu.Id)
            ) : null;
            var primeiroSindicatoPatronalUsuario = sindicatosPatronaisDoUsuarioNoDocumento is not null ? sindicatosPatronaisDoUsuarioNoDocumento.FirstOrDefault() : null;

            var assunto = tipoDocumento!.Sigla + " " +
                          "(" + documento.Database + ")" + " " +
                          (primeiroSindicatoLaboralUsuario is not null ? primeiroSindicatoLaboralUsuario!.Sigla + " " : "") +
                          (primeiroSindicatoPatronalUsuario is not null ? "com " + primeiroSindicatoPatronalUsuario!.Sigla + " " : "") +
                          (primeiroSindicatoLaboralUsuario is not null ? "- " + primeiroSindicatoLaboralUsuario.Municipio + "/" + primeiroSindicatoLaboralUsuario.Uf : "");

            InfoEmail.Assunto = assunto;
        }

        public T Build<T>()
        {
            if (typeof(T) == typeof(DocumentoAprovadoEmailDto) && InfoEmail is T resultDocumentoAprovadoEmail)
            {
                return resultDocumentoAprovadoEmail;
            }

            if (typeof(T) == typeof(DocumentoClausulasLiberadasEmailDto))
            {
                var infoEmail = new DocumentoClausulasLiberadasEmailDto
                {
                    Abrangencia = InfoEmail.Abrangencia,
                    Assunto = InfoEmail.Assunto,
                    AtividadesEconomicas = InfoEmail.AtividadesEconomicas,
                    CodigosSindicatosCliente = InfoEmail.CodigosSindicatosCliente,
                    DataSla = InfoEmail.DataSla,
                    Estabelecimentos = InfoEmail.Estabelecimentos,
                    GestaoDeChamadosUrl = InfoEmail.GestaoDeChamadosUrl,
                    LocalizacoesUnidades = InfoEmail.LocalizacoesUnidades,
                    NomeDocumento = InfoEmail.NomeDocumento,
                    SindicatosLaborais = InfoEmail.SindicatosLaborais,
                    SindicatosPatronais = InfoEmail.SindicatosPatronais,
                    Url = InfoEmail.Url,
                    VigenciaFinal = InfoEmail.VigenciaFinal,
                    VigenciaInicial = InfoEmail.VigenciaInicial,
                };

                if (infoEmail is T resultClausulaLiberadaEmail)
                {
                    return resultClausulaLiberadaEmail;
                }
            }

            throw new ArgumentException("O tipo fornecido não é suportado pelo builder");
        }
    }
}
