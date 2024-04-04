import DateFormatter from "../../../../date/date-formatter";
import { stringA, stringB, stringP } from "../../../string-elements";

export class TemplateEmailDocumentoAprovado {
  constructor({
    nomeDocumento,
    abrangencia,
    atividadeEconomica,
    sindicatosLaborais,
    sindicatosPatronais,
    vigenciaInicial,
    vigenciaFinal,
    pdfLink,
    dataSla,
    chamadoLink,
    deveMostrarSLA = true
  }) {
    this.model = {
      nomeDocumento,
      abrangencia,
      atividadeEconomica,
      sindicatosLaborais,
      sindicatosPatronais,
      vigenciaInicial,
      vigenciaFinal,
      pdfLink,
      dataSla,
      chamadoLink,
      deveMostrarSLA
    }
  }

  criar() {
    const {
      nomeDocumento,
      abrangencia,
      atividadeEconomica,
      sindicatosLaborais,
      sindicatosPatronais,
      vigenciaInicial,
      vigenciaFinal,
      pdfLink,
      dataSla,
      chamadoLink,
      deveMostrarSLA
    } = this.model

    const template = stringP({ text: `Prezado(a) Cliente ` + stringB({ text: 'Ineditta' }) }) + "\n" + "\n" +
      stringP({ text: `Informamos que o(a) ` + stringB({ text: nomeDocumento }) + ` descrito abaixo, está disponível para consulta:` }) +
      "\n" +
      stringP({ text: stringB({ text: "Abrangência: " }) + abrangencia }) +
      "\n" +
      stringP({ text: stringB({ text: "Atividade econômica: " }) + atividadeEconomica }) +
      "\n" +
      stringP({ text: stringB({ text: "Sindicato(s) Laboral(is): " }) + `${sindicatosLaborais.join('; ')}` }) +
      "\n" + (() => {
        if (!sindicatosPatronais || sindicatosPatronais.length === 0) return ''

        return stringP({ text: stringB({ text: "Sindicato(s) Patronal(is): " }) + `${sindicatosPatronais.join('; ')}` }) + "\n"
      })() +
      stringP({ text: stringB({ text: "Vigência inicial: " }) + `${DateFormatter.dayMonthYear(vigenciaInicial)}` }) +
      "\n" +
      stringP({ text: stringB({ text: "Vigência final: " }) + `${DateFormatter.dayMonthYear(vigenciaFinal)}` }) +
      "\n" +
      stringP({ text: 'Documento: arquivo ' + stringB({ text: stringA({ text: 'PDF', href: pdfLink }) }) }) +
      "\n" +
      stringP({ text: deveMostrarSLA ? ('Os demais módulos serão disponibilizados até o dia ' + stringB({ text: dataSla })) : 'Os demais módulos serão disponibilizados' }) +
      "\n" +
      "\n" +
      stringP({ text: 'Em caso de dúvidas, entre em contato conosco clicando no link: ' +
          stringB({ text: stringA({ text: 'Cadastrar chamados', href: chamadoLink }) }) }) +
      "\n" +
      "\n" +
      stringP({ text: 'Atenciosamente,' }) +
      "\n" +
      "\n" +
      stringP({ text: stringB({ text: 'Ineditta Consultoria Sindical' }) })

    return template
  }
}