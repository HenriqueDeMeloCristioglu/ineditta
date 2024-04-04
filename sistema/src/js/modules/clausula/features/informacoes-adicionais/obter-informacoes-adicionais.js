import { ApiService } from "../../../../core/index"
import { InformacaoAdicionalService } from "../../../../services"

const informacaoAdicionalService = new InformacaoAdicionalService(new ApiService())

export async function obterInformacoesAdicionais({ informacoesAdiconaisOpcoes, clausulaSelecionada }) {
  if (informacoesAdiconaisOpcoes.length <= 0) return

  const { tipoInformacaoId: grupoId, estruturaId } = informacoesAdiconaisOpcoes[0]
  const { id } = clausulaSelecionada

  const result = await informacaoAdicionalService.obterDadosCamposAdicionais({ grupoId, estruturaId, clausulaId: id })

  if (result.isFailure()) return

  return result.value
}