import $ from "jquery"
import '../../masks/jquery-mask-extensions';

// Core
import { ApiService } from "../../../core/index"

// Services
import { EstruturaClausulaService } from "../../../services"

import { InformacaoAdicionalTipo } from "../../../application/clausulas/constants/informacao-adicional-tipo"

import SelectWrapper from "../../selects/select-wrapper"
import DatePickerWrapper from "../../datepicker/datepicker-wrapper"

const apiService = new ApiService()
const estruturaClausulaService = new EstruturaClausulaService(apiService)

export const configOpcaoAdicional = (item, dado) => {
  const { id, type, data, codigo } = item

  switch (type) {
    case InformacaoAdicionalTipo.Date:
      item.input = new DatePickerWrapper(`#${id}`)
      item.command = {
        val: () => item.input.getValue()
      }

      if (dado) item.input.setValue(dado.data)

      break
    case InformacaoAdicionalTipo.Select:
      // eslint-disable-next-line no-case-declarations
      const options = data.options.split(', ')

      item.input = new SelectWrapper(`#${id}`, {
        onOpened: async () => {
          if (codigo == 170) {
            return (await estruturaClausulaService.obterSelect()).value
          }

          return await createOptions(options)
        },
      })

      if (codigo === 170) {
        item.command = {
          val: () => item.input.getSelectedOptions()
        }
      } else {
        item.command = {
          val: () => item.input.getValue()
        }
      }

      if (dado) {
        const { combo, nome } = dado

        if (codigo === 170) {
          item.input.setCurrentValue([{
            id: nome,
            description: combo
          }])
        } else {
          item.input.setCurrentValue([{
            id: combo,
            description: combo
          }])
        }
      } else {
        const content = item.command.val()
        if (content && content.length > 0) return

        const naoSeAplica = 'Não se aplica'

        if (options.find(o => o == naoSeAplica)) { 
          item.input.setCurrentValue({
            id: naoSeAplica,
            description: naoSeAplica
          })
        }

        if (codigo === 310) {
          item.input.setCurrentValue({
            id: 'Não',
            description: 'Não'
          })
        }
      }

      break
    case InformacaoAdicionalTipo.SelectMultiple:
      // eslint-disable-next-line no-case-declarations
      const optionsMultiple = data.options.split(', ')

      item.input = new SelectWrapper(`#${id}`, {
        onOpened: async () => await createOptions(optionsMultiple),
      })

      item.command = {
        val: () => (item.input.getValue()).join(', ')
      }

      if (dado) {
        const { combo } = dado

        const options = combo.split(', ')

        let selectOptions = []
        options.map(option => {
          selectOptions.push({
            id: option,
            description: option
          })
        })

        item.input.setCurrentValue(selectOptions)
      } else {
        const content = item.command.val()
        if (content && content.length > 0) return

        const naoSeAplica = 'Não se aplica'

        if (optionsMultiple.find(o => o == naoSeAplica)) { 
          item.input.setCurrentValue({
            id: naoSeAplica,
            description: naoSeAplica
          })
        }
      }

      break
    case InformacaoAdicionalTipo.Text:
      item.input = $(`#${id}`)

      item.command = {
        val: () => item.input.val()
      }

      if (dado) item.input.val(dado.texto)

      break
    case InformacaoAdicionalTipo.Hour:
      item.input = $(`#${id}`)

      item.input.maskHora()

      item.command = {
        val: () => item.input.val()
      }

      if (dado) item.input.val(dado.hora)

      break
    case InformacaoAdicionalTipo.Number:
      item.input = $(`#${id}`)

      item.command = {
        val: () => item.input.val()
      }

      if (dado) item.input.val(dado.numerico)

      break
    case InformacaoAdicionalTipo.Percent:
      item.input = $(`#${id}`)

      item.command = {
        val: () => item.input.val().replace(',', '.').replaceAll('%', '')
      }

      if (dado) {
        const value = parseFloat(dado.percentual)

        if (!isNaN(value)) {
          item.input.val(value.toFixed(2))
        }
      }

      item.input.maskPercentageWithSufix()

      break
    case InformacaoAdicionalTipo.Monetario:
      item.input = $(`#${id}`)

      item.command = {
        val: () => item.input.val().replaceAll('.', '').replace(',', '.').replace('R$ ', '')
      }

      if (dado && dado.numerico) {
        item.input.val(dado.numerico.toFixed(2))
      }

      item.input.maskMonetario()
      item.input.focus()

      break
    default:
      item.input = $(`#${id}`)

      item.command = {
        val: () => item.input.val()
      }

      if (dado) item.input.val(dado.descricao)

      break
  }
}

async function createOptions(options) {
  return await Promise.resolve(options.map(item => {
    return {
      id: item,
      description: item
    }
  }))
}