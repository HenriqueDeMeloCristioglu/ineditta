import { ApiService } from "../../../../core/index"
import { TipoDocService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const tipoDocService = new TipoDocService(new ApiService())

export function tipoDocumentoSelect(options = null) {
  let selector = '#tipo_documento'

  if (options && options.selector) {
    selector = options.selector
  }

  return new SelectWrapper(selector, {
    options: { placeholder: "Selecione", multiple: true },
    onOpened: async () => await tipoDocService.obterSelectPorTipos({ processado: true })
  })
}