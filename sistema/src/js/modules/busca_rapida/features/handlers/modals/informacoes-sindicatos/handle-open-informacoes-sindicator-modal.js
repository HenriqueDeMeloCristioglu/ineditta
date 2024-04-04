import $ from 'jquery'
import NotificationService from '../../../../../../utils/notifications/notification.service'
import { obterInformacoesSindicatoPorId } from '../../../obter-dados'
import { dataTableInfoDiretoriaTb } from '../../../../components'
import { limparModalInfo } from '../../../actions'
import { LinkFormatter, LinkFormatterType } from '../../../../../../utils/links'

export async function handleOpenInformacoesSindicatosModal(context) {
  const id = $("#sind-id-input").val()
  const tipoSindicato = $("#tipo-sind-input").val()

  if (id) {
    const result = await obterInformacoesSindicatoPorId(id, tipoSindicato)
    if (result.isFailure()) {
      return NotificationService.error({ title: 'Erro ao obter informações do sindicato!', message: result.error })
    }

    const data = result.value

    limparModalInfo()

    $("#info-sigla").val(data.sigla)
    $("#info-cnpj").maskCNPJ().val(data.cnpj).trigger("input")
    $("#info-razao").val(data.razaoSocial)
    $("#info-denominacao").val(data.denominacao)
    $("#info-cod-sindical").val(data.codigoSindical)
    $("#info-uf").val(data.uf)
    $("#info-municipio").val(data.municipio)
    $("#info-logradouro").val(data.logradouro)
    $("#info-telefone1").maskCelPhone().val(data.telefone1).trigger("input")
    $("#info-telefone2").maskCelPhone().val(data.telefone2).trigger("input")
    $("#info-telefone3").maskCelPhone().val(data.telefone3).trigger("input")
    $("#info-ramal").val(data.ramal)
    $("#info-enquadramento").val(data.contatoEnquadramento)
    $("#info-negociador").val(data.contatoNegociador)
    $("#info-contribuicao").val(data.contatoContribuicao)
    $("#info-email1")
      .val(data.email1)
      .attr("style", data.email1 ? "cursor: pointer" : null)
    $("#info-email1-link").attr("href", `mailto:${data.email1}`)
    $("#info-email2")
      .val(data.email2)
      .attr("style", data.email2 ? "cursor: pointer" : null)
    $("#info-email2-link").attr("href", `mailto:${data.email2}`)
    $("#info-email3")
      .val(data.email3)
      .attr("style", data.email3 ? "cursor: pointer" : null)
    $("#info-email3-link").attr("href", `mailto:${data.email3}`)
    $("#info-twitter")
      .val(data.twitter)
      .attr("style", data.twitter ? "cursor: pointer" : null)
    $("#info-twitter-link").attr("href", LinkFormatter.midiasSociais(data.twitter, LinkFormatterType.Twitter))
    $("#info-facebook")
      .val(data.facebook)
      .attr("style", data.facebook ? "cursor: pointer" : null)
    $("#info-facebook-link").attr("href", LinkFormatter.midiasSociais(data.facebook, LinkFormatterType.Facebook))
    $("#info-instagram")
      .val(data.instagram)
      .attr("style", data.instagram ? "cursor: pointer" : null)
    $("#info-instagram-link").attr("href", LinkFormatter.midiasSociais(data.instagram, LinkFormatterType.Intagram))
    $("#info-site")
      .val(data.site)
      .attr("style", data.site ? "cursor: pointer" : null)
    $("#info-site-link").attr("href", LinkFormatter.midiasSociais(data.site, LinkFormatterType.Site))
    $("#info-data-base").val(data.dataBase)
    $("#info-ativ-econ").val(data.atividadeEconomica)
    $("#info-federacao-nome").val(data?.federacao?.nome)
    $("#info-federacao-cnpj")
      .maskCNPJ()
      .val(data?.federacao?.cnpj)
      .trigger("input")
    $("#info-confederacao-nome").val(data?.confederacao?.nome)
    $("#info-confederacao-cnpj")
      .maskCNPJ()
      .val(data?.confederacao?.cnpj)
      .trigger("input")
    $("#info-central-sind-nome").val(data?.centralSindical?.nome)
    $("#info-central-sind-cnpj")
      .maskCNPJ()
      .val(data?.centralSindical?.cnpj)
      .trigger("input")

    $("#direct-comparativo-btn").attr("href", `consultaclausula.php?sindId=${id}&tipoSind=${tipoSindicato}&comparativo=${true}&sigla=${data.sigla}`)
    $("#direct-calendarios-btn").attr("href", `calendario_sindical.php?sindId=${id}&tipoSind=${tipoSindicato}&sigla=${data.sigla}`)
    $("#direct-documentos-btn").attr("href", `consulta_documentos.php?sindId=${id}&tipoSind=${tipoSindicato}&sigla=${data.sigla}`)
    $("#direct-formulario-aplicacao-btn").attr("href", `formulario_comunicado.php?sindId=${id}&tipoSind=${tipoSindicato}&sigla=${data.sigla}`)
    $("#direct-gerar-excel-btn").attr("href", `geradorCsv.php?sindId=${id}&tipoSind=${tipoSindicato}&sigla=${data.sigla}`)
    $("#direct-comparativo-mapa-btn").attr("href", `comparativo.php?sindId=${id}&tipoSind=${tipoSindicato}&sigla=${data.sigla}`)
    $("#direct-relatorio-negociacoes-btn").attr("href", `relatorio_negociacoes.php?sigla=${data.sigla}`)

    await dataTableInfoDiretoriaTb({
      context,
      params: {
        sindicatoId: id,
        tipoSindicato
      }
    })
  }
}
