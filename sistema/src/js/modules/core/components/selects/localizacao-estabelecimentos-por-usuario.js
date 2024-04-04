import SelectWrapper from "../../../../utils/selects/select-wrapper"
import { obterLocalidadesSelectOption } from "../../../busca_rapida"

export function localizacaoEstabelecimentosPorUsuarioSelect({
  selector = '#localidade',
  onChange,
  markOptionAsSelectable
}) {
  return new SelectWrapper(selector, {
    options: { placeholder: "Selecione", multiple: true },
    onChange: async (data) => await onChange(data),
    onOpened: async () => await obterLocalidadesSelectOption(),
    markOptionAsSelectable,
  })
}