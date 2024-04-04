import { stringA, stringB, stringP } from "../../../../string-elements";

export class TemplateEmailAcompanhamentoCctReajusteSalarial {
  constructor({ localidade }) {
    this.model = {
      localidade
    }
  }

  criar() {
    const { localidade } = this.model

    const br = "\n"
    const template = stringP({ text: "Prezado(a) Cliente," }) + br +
      stringP({
        text: `A Ata/Circular Reajuste Salarial (${localidade}), que dispõe sobre o fechamento da CCT está disponível no ` + br +
          stringA({ href: `${window.URL}`, text: "Sistema Ineditta.", target: "_blank" }) + br +
          stringP({ text: "Continuaremos acompanhando a assinatura/homologação do documento e qualquer novidade comunicaremos." }) + br +
          stringP({ text: "Em caso de dúvidas, entre em contato com a nossa Equipe respondendo esse e-mail ou nos telefones (11) 2321-3790 Ramal 1006 ao 1009." }) + br +
          stringP({ text: "Atenciosamente," }) + br +
          stringP({ text: stringB({ text: "Operação Acompanhamento | Ineditta Consultoria Sindical" }) }) + br +
          stringP({ text: stringB({ text: "Telefone:</b> (11) 2321-3790 Ramal 1006 / 1007 / 1008 / 1009" }) }),
      })

    return template
  }
}