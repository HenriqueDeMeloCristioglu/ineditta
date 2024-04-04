import { ApiService } from "../../../../core/index"
import { CnaeService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"
import $ from 'jquery'

const cnaeService = new CnaeService(new ApiService())

export function cnaePorGrupoEmpresaEstabelecimentoSelect({
  selector = '#atividadeEconomicaSelect',
  gruposEconomicoSelector = '#grupo',
  empresaSelector = '#matriz',
  estabelecimentoSelector = '#unidade',
  onChange = null
}) {
  return new SelectWrapper(selector, {
    options: { placeholder: 'Selecione', multiple: true },
    onOpened: async () => {
        const options = {
          gruposEconomicosIds: $(gruposEconomicoSelector).val() ?? null,
          matrizesIds: $(empresaSelector).val() ?? null,
          clientesUnidadesIds: $(estabelecimentoSelector).val() ?? null
        }

        return await cnaeService.obterSelectPorUsuario(options)
    },
    onChange
  })
}