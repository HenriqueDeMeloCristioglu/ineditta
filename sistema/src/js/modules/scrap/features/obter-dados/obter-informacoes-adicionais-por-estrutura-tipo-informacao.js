import Result from "../../../../core/result"
import { useInformacaoAdicionalService } from "../../../core/hooks"

const informacaoAdicionalService = useInformacaoAdicionalService()

export async function obterInformacoesAdicionaisPorEstruturaTipoInformcao({
  tipoInformacaoId,
  estruturaId
}) {
  const result = await informacaoAdicionalService.obterCamposAdicionais(tipoInformacaoId, estruturaId)

  if (result.isFailure()) {
    return Result.failure('Erro ao buscar informações adicionais')
  }

  return Result.success(result.value)
}
