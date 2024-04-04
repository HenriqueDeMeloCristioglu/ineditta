import { ApiService } from "../../../../core/index"
import { MatrizService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const matrizService = new MatrizService(new ApiService())

export function empresaPorUsuarioSelect({
  selector = '#matriz',
  parentId = "#grupo",
  onChange,
  isIneditta,
  isGrupoEconomico
}) {
  return new SelectWrapper(selector, {
    options: {
      placeholder: "Selecione",
      multiple: true,
    },
    parentId,
    onChange: async (data) => await onChange(data),
    onOpened: async (grupoEconomicoId) => await matrizService.obterSelectPorUsuario(grupoEconomicoId),
    markOptionAsSelectable: isIneditta || isGrupoEconomico ? () => false : () => true,
  })
}
