import { stringB, stringP } from "../../../../string-elements";

export class TemplateEmailAcompanhamentoCctAssembleiaPatronal {
  constructor({
    nomeTipoDocumento,
    estados,
    cidades,
    cnaes,
    dataBase
  }) {
    this.model = {
      nomeTipoDocumento,
      estados,
      cidades,
      cnaes,
      dataBase
    }
  }

  criar() {
    const {
      nomeTipoDocumento,
      estados,
      cidades,
      cnaes,
      dataBase
    } = this.model

    const br = "\n"
    const template = stringP({ text: "Prezados(as)," }) + br +
      stringP({ text: `Gostaríamos de confirmar se as negociações referentes a ${nomeTipoDocumento} abaixo já iniciaram:` }) + br +
      stringP({ text: `Estados: ${estados}` }) + br +
      stringP({ text: `Cidades: ${cidades}` }) + br +
      stringP({ text: `Atividade Econômica: ${cnaes}` }) + br +
      stringP({ text: `Data-Base: ${dataBase}` }) + br +
      stringP({ text: "Possuem data prevista para assembleia com a categoria patronal? Poderiam nos informar?" }) + br +
      stringP({ text: "Atenciosamente," }) + br +
      stringP({ text: stringB({ text: "Operação Acompanhamento | Ineditta Consultoria Sindical" }) }) + br +
      stringP({ text: stringB({ text: "Telefone: (11) 2321-3790 Ramal 1006 / 1007 / 1008 / 1009" }) })

    return template
  }
}