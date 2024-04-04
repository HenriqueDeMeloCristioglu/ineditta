import { ApiService } from "../../../../core/index"
import { EstruturaClausulaService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const estruturaClausulaService = new EstruturaClausulaService(new ApiService())

export function assuntoSelect() {
  return new SelectWrapper('#assunto', { onOpened: async () => (await estruturaClausulaService.obterSelect()).value })
}