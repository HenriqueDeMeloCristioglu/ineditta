import { stringB, stringP } from "../../../../string-elements";

export class TemplateEmailAcompanhamentoCctNegociacaoCct {
  constructor({
    nomeTipoDocumento,
    localidades,
    dataBase,
    cnaes
  }) {
    this.model = {
      nomeTipoDocumento,
      localidades,
      dataBase,
      cnaes
    }
  }

  criar() {
    const {
      nomeTipoDocumento,
      localidades,
      dataBase,
      cnaes
    } = this.model

    const br = "\n"
    const template = stringP({ text: "Prezados(as)," }) + br +
      stringP({ text: `Gostaríamos de confirmar se as negociações referentes a ${nomeTipoDocumento} abaixo já iniciaram:` }) + br +
      stringP({ text: `Gostaria de saber como estão as negociações da CCT ${localidades} com data-base: ${dataBase} para a(s) atividade econômica(s): ${cnaes}.` }) + br +
      stringP({ text: "Tem alguma previsão de reunião entre os sindicatos?" }) + br +
      stringP({ text: "Desde já agradeço e fico no aguardo." }) + br +
      stringP({ text: "Atenciosamente," }) + br +
      stringP({ text: stringB({ text: "Operação Acompanhamento | Ineditta Consultoria Sindical" }) }) + br +
      stringP({ text: stringB({ text: "Telefone: (11) 2321-3790 Ramal 1005 / 1006 / 1007 / 1008 / 1009" }) })

    return template
  }
}