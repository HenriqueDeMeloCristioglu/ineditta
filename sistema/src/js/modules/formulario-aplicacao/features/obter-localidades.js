import { ApiService } from "../../../core/index"
import { LocalizacaoService } from "../../../services"

const apiService = new ApiService()
const localizacaoService = new LocalizacaoService(apiService)

export async function obterLocalidades() {
  const municipios = await localizacaoService.obterSelectPorUsuario()
  const regioes = await localizacaoService.obterSelectRegioes(true)

  const localidades = municipios?.map((municipio) => ({ id: `municipio:${municipio.id}`, description: municipio.description })) ?? []

  if (regioes?.length > 0) {
    localidades.push(...regioes.map(regiao => ({ id: `uf:${regiao.description}`, description: regiao.description })))
  }

  return localidades
}