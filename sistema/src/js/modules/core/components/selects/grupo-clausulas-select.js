import { ApiService } from "../../../../core/index"
import { GrupoClausulaService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const grupoClausulaService = new GrupoClausulaService(new ApiService())

export function grupoClausulasSelect(options) {
  let selector = '#grupo_clausulas'

  if (options && options.selector) {
    selector = options.selector
  }

  return new SelectWrapper(selector, {
    onOpened: async () => (await grupoClausulaService.obterSelect()).value,
    sortable: true,
  })
}