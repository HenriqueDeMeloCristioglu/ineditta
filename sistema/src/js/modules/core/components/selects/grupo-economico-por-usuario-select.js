import { ApiService } from "../../../../core/index"
import { GrupoEconomicoService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const grupoEconomicoService = new GrupoEconomicoService(new ApiService())

export function grupoEconomicoPorUsuarioSelect({
  selector = '#grupo',
  isIneditta,
  onChange
}) {
  return new SelectWrapper(selector, {
    options: { placeholder: "Selecione" },
    onChange: async (data) => await onChange(data),
    onOpened: async () => await grupoEconomicoService.obterSelectPorUsuario(),
    markOptionAsSelectable: isIneditta ? () => false : () => true,
  })
}
