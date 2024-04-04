/* eslint-disable no-case-declarations */
import $ from 'jquery'
import '../../masks/jquery-mask-extensions';

import { InformacaoAdicionalTipo } from "../../../application/clausulas/constants/informacao-adicional-tipo"
import { input, select, textarea } from "../elements"
import { Generator } from '../../generator'
import DatepickerWrapper from "../../datepicker/datepicker-wrapper"
import SelectWrapper from '../../selects/select-wrapper';
import DateFormatter from '../../date/date-formatter';

export class JOpcaoAdicional {
  constructor({ tipo, dado, codigo }) {
    const id = Generator.id()

    this.id = id
    this.tipo = tipo
    this.dado = dado
    this.codigo = codigo
    this.content = null

    this.configure({
      tipo,
      dado,
      id
    })
  }

  configure({ tipo, dado, id }) {
    let baseClass = 'form-control info-adicional w-100'

    switch (tipo) {
      case InformacaoAdicionalTipo.Date:
        this.content = input({
          type: 'text',
          id,
          className: baseClass + 'date-input',
          placeholder: '00/00/0000',
          content: dado && DateFormatter.dayMonthYear(dado.data)
        })
        break
      case InformacaoAdicionalTipo.Number:
        this.content = input({
          type: 'number',
          id,
          className: baseClass + 'number-input',
          content: dado && dado.numerico
        })
        break
      case InformacaoAdicionalTipo.Percent:
        this.content = input({
          type: 'text',
          id,
          className: baseClass + 'percent-input',
          content: dado && (() => {
            const value = parseFloat(dado.percentual)

            if (!isNaN(value)) return value.toFixed(2)
          })()
        })
        break
      case InformacaoAdicionalTipo.Hour:
        this.content = input({
          type: 'text',
          id,
          className: baseClass + 'hora-input',
          content: dado && dado.hora
        })
        break
      case InformacaoAdicionalTipo.Text:
        this.content = input({
          type: 'text',
          id,
          className: baseClass + 'valor-input',
          content: dado && dado.texto
        })
        break
      case InformacaoAdicionalTipo.Monetario:
        this.content = input({
          type: 'text',
          id,
          className: baseClass + 'valor-input',
          content: (dado && dado.numerico) && dado.numerico.toFixed(2)
        })
        break
      case InformacaoAdicionalTipo.Select:
        this.content = select({
          type: 'text',
          id,
          className: baseClass + 'valor-input',
          placeholder: 'Selecione'
        })
        break
      case InformacaoAdicionalTipo.SelectMultiple:
        this.content = select({
          type: 'text',
          id,
          className: baseClass + 'valor-input',
          placeholder: 'Selecione',
          multiple: true
        })
        break
      default:
        this.content = textarea({
          id,
          className: baseClass,
          content: (dado && dado !== null) && dado.descricao
        })
        break
    }
  }

  init() {
    switch (this.tipo) {
      case InformacaoAdicionalTipo.Date:
        this.content = new DatepickerWrapper(`#${this.id}`)
        break;
      case InformacaoAdicionalTipo.Percent:
        this.content.maskPercentageWithSufix()
        break
      case InformacaoAdicionalTipo.Hour:
        this.content.maskHora()
        break
      case InformacaoAdicionalTipo.Monetario:
        this.content.maskMonetario()
        this.content.focus()
        break
      case InformacaoAdicionalTipo.Select:
        this.content = new SelectWrapper(`#${this.id}`, {
          onOpened: async () => await this.createOptions(this.dado.combo.opcoes)
        })

        this.content.setCurrentValue([{
          id: this.dado.combo.valor[0],
          description: this.dado.combo.valor[0]
        }])

        this.codigo == 170 && this.content.disable()
        break
      case InformacaoAdicionalTipo.SelectMultiple:
        this.content = new SelectWrapper(`#${this.id}`, {
          onOpened: async () => await this.createOptions(this.dado.combo.opcoes)
        })

        let options = [];

        if (this.dado.combo.valor) {
          for (const item of this.dado.combo.valor) {
            if (item) {
              options.push({
                id: item,
                description: item
              })
            }
          }
        }

        options.length > 0 && this.content.setCurrentValue(options)
        break
      default:
        break;
    }
  }

  val() {
    switch (this.tipo) {
      case InformacaoAdicionalTipo.Date:
        return this.content.getValue()
      case InformacaoAdicionalTipo.Monetario:
        return parseFloat($(`#${this.id}`).val().replaceAll('.', '').replaceAll(',', '.').replace('R$ ', ''))
      case InformacaoAdicionalTipo.Number:
        return parseFloat($(`#${this.id}`).val().replaceAll('.', '').replaceAll(',', '.').replace('R$ ', ''))
      case InformacaoAdicionalTipo.Select:
        return this.content.getValue()
      case InformacaoAdicionalTipo.SelectMultiple:
        return this.content.getValue()
      case InformacaoAdicionalTipo.Percent:
        return $(`#${this.id}`).val().replace(',', '.').replaceAll('%', '')
      default:
        return $(`#${this.id}`).val()
    }
  }

  focus() {
    $(`#${this.id}`).focus()
  }

  async createOptions(options) {
    return await Promise.resolve(options.map(item => {
      return {
        id: item,
        description: item
      }
    }))
  }
}