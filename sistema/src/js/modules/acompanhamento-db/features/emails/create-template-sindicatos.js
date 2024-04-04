import $ from 'jquery'
import { TemplateEmailAcompanhamentoCctAssembleiaPatronal, TemplateEmailAcompanhamentoCctNegociacaoCct } from "../../../../utils/components/emails/templates"
import { EmailsAcompanhametoCct } from './enums'

export function createTemplateSindicato({ acompanhamentoSelecionado, template }) {
  let html = ""

  const cnaes = []
  acompanhamentoSelecionado.cnaes.forEach((cnae) => cnaes.push(cnae.descricao))
  
  const cidade = []
  const estado = []
  const localidades = []
  acompanhamentoSelecionado?.sindicatosLaborais.map(({ uf, municipio }) => {
    if (cidade.includes(uf)) return
    if (estado.includes(uf)) return

    cidade.push(municipio)
    estado.push(uf)
    localidades.push(`${municipio}/${uf}`)
  })

  if (template == EmailsAcompanhametoCct.Sindicatos.AssembleiaPatronal) {
    $("#assunto-input").val(`Acompanhamento Ineditta ${acompanhamentoSelecionado?.id}`)

    const templateEmailAcompanhamentoCctAssembleiaPatronal = new TemplateEmailAcompanhamentoCctAssembleiaPatronal({
      nomeTipoDocumento: acompanhamentoSelecionado.nomeTipoDocumento,
      estados: estado.join(", "),
      cidades: cidade.join(", "),
      cnaes: acompanhamentoSelecionado.atividadesEconomicas,
      dataBase: acompanhamentoSelecionado.dataBase
    })

    html = templateEmailAcompanhamentoCctAssembleiaPatronal.criar()
  }

  if (template == EmailsAcompanhametoCct.Sindicatos.NegociaçãoCCT) {
    $("#assunto-input").val(`Acompanhamento Ineditta ${acompanhamentoSelecionado.id}`)

    const templateEmailAcompanhamentoCctAssembleiaPatronal = new TemplateEmailAcompanhamentoCctNegociacaoCct({
      nomeTipoDocumento: acompanhamentoSelecionado.nomeTipoDocumento,
      cnaes: acompanhamentoSelecionado.atividadesEconomicas,
      localidades,
      dataBase: acompanhamentoSelecionado.dataBase
    })

    html = templateEmailAcompanhamentoCctAssembleiaPatronal.criar()
  }

  $("#msg-input").val(html)
}