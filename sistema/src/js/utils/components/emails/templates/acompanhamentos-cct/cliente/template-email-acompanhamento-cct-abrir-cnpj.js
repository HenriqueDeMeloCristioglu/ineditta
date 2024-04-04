import { stringB, stringP } from "../../../../string-elements";

export class TemplateEmailAcompanhamentoCctAbrirCnpj {
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
      stringP({ text: `Data-Base: ${dataBase}` }) +
      stringP({ text: "O Sindicato Patronal nos compartilha informações sobre a CCT apenas para empresas associadas, contribuintes ou com a abertura do nome da empresa ou CNPJ, podemos informar?" }) +
      stringP({ text: "Atenciosamente," }) + br +
      stringP({ text: stringB({ text: "Operação Acompanhamento | Ineditta Consultoria Sindical" }) }) + br +
      stringP({ text: stringB({ text: "Telefone:</b> (11) 2321-3790 Ramal 1006 / 1007 / 1008 / 1009" }) });

    return template
  }
}