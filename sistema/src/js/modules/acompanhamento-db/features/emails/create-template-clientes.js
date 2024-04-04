import $ from 'jquery'
import { TemplateEmailAcompanhamentoCctAbrirCnpj, TemplateEmailAcompanhamentoCctClienteSemContatoSindicato, TemplateEmailAcompanhamentoCctReajusteSalarial } from '../../../../utils/components/emails/templates'
import { EmailsAcompanhametoCct } from './enums'

export function setTemplateCliente({ acompanhamentoSelecionado, template }) {
  let html = ""

  const cidade = []
  const estado = []
  const localidade = []
  acompanhamentoSelecionado?.sindicatosLaborais.map(({ uf, municipio }) => {
    if (cidade.includes(uf)) return
    if (estado.includes(uf)) return

    cidade.push(municipio)
    estado.push(uf)
    localidade.push(`${municipio}/${uf}`)
  })

  $("#assunto-input-cli").val("Acompanhamento Ineditta " + acompanhamentoSelecionado?.id)

  switch (template) {
    case EmailsAcompanhametoCct.Clientes.AtaCircularReajusteSalarial:
      const templateEmailAcompanhamentoCctReajusteSalarial = new TemplateEmailAcompanhamentoCctReajusteSalarial({ localidade })
      html = templateEmailAcompanhamentoCctReajusteSalarial.criar()
      break
    case EmailsAcompanhametoCct.Clientes.ContatoSindicato:
      const templateEmailAcompanhamentoCctClienteSemContatoSindicato = new TemplateEmailAcompanhamentoCctClienteSemContatoSindicato({
        estado: estado.join(', '),
        cidade: cidade.join(', '),
        cnaes: acompanhamentoSelecionado.atividadesEconomicas,
        dataBase: acompanhamentoSelecionado.dataBase
      })
      html = templateEmailAcompanhamentoCctClienteSemContatoSindicato.criar()
      break
    case EmailsAcompanhametoCct.Clientes.AbrirCnpjENomeEmpesa:
      const templateEmailAcompanhamentoCctAbrirCnpj = new TemplateEmailAcompanhamentoCctAbrirCnpj({
        estado: estado.join(', '),
        cidade: cidade.join(', '),
        cnaes: acompanhamentoSelecionado.atividadesEconomicas,
        dataBase: acompanhamentoSelecionado.dataBase
      })
      html = templateEmailAcompanhamentoCctAbrirCnpj.criar()
      break
    default:
      break
  }

  $("#msg-input-cli").val(html)
}