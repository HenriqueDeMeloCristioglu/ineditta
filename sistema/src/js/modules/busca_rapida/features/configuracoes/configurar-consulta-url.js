import { obterParametrosUrl } from "../../../core"
import { handleFiltrarClausulas } from "../handlers"

export async function configurarConsultaUrl(context) {
  const { datas } = context

  const urlParams = obterParametrosUrl()

  if (!urlParams.has("iddoc")) {
    return
  }

  datas.idDoc = urlParams.get("iddoc")

  await handleFiltrarClausulas(context)
}
