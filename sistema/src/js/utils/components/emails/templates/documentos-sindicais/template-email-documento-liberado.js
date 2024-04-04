import DateFormatter from "../../../../date/date-formatter";
import { stringA, stringB, stringP } from "../../../string-elements";

export class TemplateEmailDocumentoLiberado {
  constructor(model) {
    this.model = model
  }

  criar() {
    const {
      documentoSelecionado,
      cnaes,
      sindicatosLaborais,
      sindicatosPatronais,
      pdfLink,
      chamadoLink,
      abrangencia
    } = this.model

    const template = stringP({ text: `Prezado(a) Cliente ` + stringB({ text: 'Ineditta' }) }) + "\n" + "\n" +
      stringP({
        text: `As informações dos módulos: ` +
          stringB({ text: "Cláusulas, Comparativo de cláusulas, Calendário Sindical, Mapa Sindical" }) + ` do documento ${documentoSelecionado.nome} descrito abaixo, estão disponíveis para consulta.`,
      }) +
      "\n" +
      stringP({ text: stringB({ text: "Abrangência: " }) + abrangencia }) +
      "\n" +
      stringP({ text: stringB({ text: "Atividade econômica: " }) + `${cnaes.join(', ')}` }) +
      "\n" +
      stringP({ text: stringB({ text: "Sindicato(s) Laboral(is): " }) + `${sindicatosLaborais.join('; ')}` }) +
      "\n" + (() => {
        if (!sindicatosPatronais || sindicatosPatronais.length === 0) return ''

        return stringP({ text: stringB({ text: "Sindicato(s) Patronal(is): " }) + `${sindicatosPatronais.join('; ')}` }) + "\n"
      })() +
      stringP({ text: stringB({ text: "Vigência inicial: " }) + `${DateFormatter.dayMonthYear(documentoSelecionado.validadeInicial)}` }) +
      "\n" +
      stringP({ text: stringB({ text: "Vigência final: " }) + `${DateFormatter.dayMonthYear(documentoSelecionado.validadeFinal)}` }) +
      "\n" +
      "\n" +
      "\n" +
      stringP({ text: 'Documento: arquivo ' + stringB({ text: stringA({ text: 'PDF', href: pdfLink }) }) }) +
      "\n" +
      "\n" +
      stringP({
        text: 'Em caso de dúvidas, entre em contato conosco clicando no link: ' +
          stringB({ text: stringA({ text: 'Cadastrar chamados', href: chamadoLink }) }),
      }) +
      "\n" +
      "\n" +
      stringP({ text: 'Atenciosamente,' }) +
      "\n" +
      "\n" +
      stringP({ text: stringB({ text: 'Ineditta Consultoria Sindical' }) })

    return template
  }
}