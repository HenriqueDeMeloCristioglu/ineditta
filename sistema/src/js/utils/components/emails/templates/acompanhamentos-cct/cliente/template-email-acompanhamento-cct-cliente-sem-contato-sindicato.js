import { stringB, stringP } from "../../../../string-elements";

export class TemplateEmailAcompanhamentoCctClienteSemContatoSindicato {
  constructor({
    estado,
    cidade,
    cnaes,
    dataBase
  }) {
    this.model = {
      estado,
      cidade,
      cnaes,
      dataBase
    }
  }

  criar() {
    const {
      estado,
      cidade,
      cnaes,
      dataBase
    } = this.model

    const br = "\n"
    const template = stringP({ text: "Prezados(as)," }) + br +
      stringP({ text: "Não estamos conseguindo falar com os sindicatos por telefone, referente o acompanhamento da Convenção Coletiva abaixo:" }) + br +
      stringP({ text: `Estado: ${estado}` }) + br +
      stringP({ text: `Cidade: ${cidade}` }) + br +
      stringP({ text: `Atividade Econômica: ${cnaes}` }) + br +
      stringP({ text: `Data-Base: ${dataBase}` }) + br +
      stringP({ text: "Porém, continuamos acompanhando os canais: Sites e Mediador." }) + br +
      stringP({ text: "Caso tenham algum contato de comunicação com o sindicato que facilite o nosso acesso, se possível, compartilhe conosco!" }) + br +
      stringP({ text: "Atenciosamente," }) + br +
      stringP({ text: stringB({ text: "Operação Acompanhamento | Ineditta Consultoria Sindical" }) }) + br +
      stringP({ text: stringB({ text: "Telefone:</b> (11) 2321-3790 Ramal 1006 / 1007 / 1008 / 1009" }) })

    return template
  }
}