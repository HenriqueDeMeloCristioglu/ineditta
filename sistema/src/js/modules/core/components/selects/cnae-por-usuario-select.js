import { ApiService } from "../../../../core/index"
import { CnaeService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const cnaeService = new CnaeService(new ApiService())

export function cnaePorUsuarioSelect({
  selector = "#cnaes"
}) {
  return new SelectWrapper(selector, {
    options: { placeholder: "Selecione" },
    onOpened: async () => await cnaeService.obterSelectPorUsuario()
  })
}