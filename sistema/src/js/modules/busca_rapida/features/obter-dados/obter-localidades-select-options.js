import { ApiService } from "../../../../core/index"
import { LocalizacaoService } from "../../../../services"

const localizacaoService = new LocalizacaoService(new ApiService())

export async function obterLocalidadesSelectOption() {
  const municipios = await localizacaoService.obterSelectPorUsuario()
  const regioes = await localizacaoService.obterSelectRegioes(true)

  const options = municipios?.map((municipio) => ({
      id: `municipio:${municipio.id}`,
      description: municipio.description,
    })) ?? []

  if (regioes?.length > 0) {
    options.push(
      ...regioes.map((regiao) => ({
        id: `uf:${regiao.description}`,
        description: regiao.description,
      }))
    )
  }

  return options
}