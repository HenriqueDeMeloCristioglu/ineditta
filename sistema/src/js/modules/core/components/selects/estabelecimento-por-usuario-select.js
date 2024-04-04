import { ApiService } from "../../../../core/index"
import { ClienteUnidadeService } from "../../../../services"
import SelectWrapper from "../../../../utils/selects/select-wrapper"

const clienteUnidadeService = new ClienteUnidadeService(new ApiService())

export function estabelecimentoPorUsuarioSelect({
  selector = '#unidade',
  parentId = "#matriz",
  onChange,
  isEstabelecimento
}) {
  return new SelectWrapper(selector, {
    options: {
      placeholder: "Selecione",
      multiple: true,
    },
    parentId,
    onChange: async (data) => await onChange(data),
    onOpened: async (matrizId) => await clienteUnidadeService.obterSelectPorUsuario(matrizId),
    markOptionAsSelectable: isEstabelecimento ? () => true : () => false,
  })
}

