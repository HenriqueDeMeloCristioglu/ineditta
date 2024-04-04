import Result from "../../../../../core/result"
import { GrupoInformacoesAdicionais } from "../../../components/informacoes-adicionais/grupo-infromacoes-adicionais/grupo-informacoes-adicionais"
import { obterInformacoesAdicionaisGrupoPorClausula } from "../../obter-dados"

export async function handleChangeEstruturaClausula({ id }, context) {
  const result = await obterInformacoesAdicionaisGrupoPorClausula(parseInt(id))
  if (result.isFailure()) {
    return Result.failure()
  }

  const grupoInformacoesAdicionais = new GrupoInformacoesAdicionais(result.value)
  grupoInformacoesAdicionais.create(context)
}