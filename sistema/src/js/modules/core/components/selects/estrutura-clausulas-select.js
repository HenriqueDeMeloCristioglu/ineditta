import { ApiService } from "../../../../core/index"
import { EstruturaClausulaService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const estruturaClausulaService = new EstruturaClausulaService(new ApiService())

export function estruturaClausulasSelect(options = null) {
  let selector = '#estrutura_clausula'
  if (options && options.selector) {
    selector = options.selector
  }
  
  let parentId = '#grupo_clausulas'
  if (options && options.parentId) {
    parentId = options.parentId
  }

  return new SelectWrapper(selector, {
    onOpened: async (grupoClausula) => (await estruturaClausulaService.obterSelectPorGrupo(grupoClausula, false, options.filter)).value,
    sortable: true,
    parentId,
  })  
}