import { InformacaoAdicionalTipo } from "../../../application/clausulas/constants/informacao-adicional-tipo"
import { createInfoAdicionalInput, createInfoAdicionalSelect } from "./info-adicional-form"
import { stringTextArea } from "../string-elements"
import { Generator } from '../../generator'
import { createInfoAdicionalSelectMultiple } from "./info-adicional-form/create-info-adicional-select-multiple"

export const stringSearchOpcaoAdicional = ({ input }) => {
  const id = Generator.id()

  switch (input) {
    case InformacaoAdicionalTipo.Date:
      return responseOption({ id, content: createInfoAdicionalInput({ id, className: 'date-input', placeholder: '00/00/0000' }) })
    case InformacaoAdicionalTipo.Number:
      return responseOption({ id, content: createInfoAdicionalInput({ type: 'number', id, className: 'number-input' }) })
    case InformacaoAdicionalTipo.Percent:
      return responseOption({ id, content: createInfoAdicionalInput({ id, className: 'percent-input' }) })
    case InformacaoAdicionalTipo.Hour:
      return responseOption({ id, content: createInfoAdicionalInput({ id, className: 'hora-input' }) })
    case InformacaoAdicionalTipo.Text:
      return responseOption({ id, content: createInfoAdicionalInput({ id }) })
    case InformacaoAdicionalTipo.Monetario:
      return responseOption({ id, content: createInfoAdicionalInput({ id, className: 'valor-input' }) })
    case InformacaoAdicionalTipo.Select:
      return responseOption({ id, content: createInfoAdicionalSelect({ id }) })
    case InformacaoAdicionalTipo.SelectMultiple:
      return responseOption({ id, content: createInfoAdicionalSelectMultiple({ id, placeholder: 'Selecione' }) })
    default:
      return responseOption({ id, content: stringTextArea({ id, className: 'form-control info-adicional', rows: '1' }) })
  }
}

function responseOption({ id, content }) {
  return {
    id,
    content
  }
}