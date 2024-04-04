import { ApiService } from "../../../core/index"
import { LocalizacaoService } from "../../../services"

const apiService = new ApiService()
const localizacaoService = new LocalizacaoService(apiService)

export async function obterLocalidades({
  grupoEconomicoId,
  matrizesIds,
  clientesUnidadesIds,
  tipoLocalidade,
  localizacoesPorAcompanhamentos
}) {
  const localizacoes = await localizacaoService.obterSelectPorTipoLocalidade({
    clientesUnidadesIds,
    matrizesIds,
    grupoEconomicoId,
    tipoLocalidade,
    localizacoesPorAcompanhamentos
  },true)

  return localizacoes?.map((loc) => ({ id: loc.id, description: loc.description })) ?? []
}