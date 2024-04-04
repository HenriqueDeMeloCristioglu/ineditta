import { closeModal } from "../../../../../utils/modals/modal-wrapper"
import { reprocessarDocumento } from "../../actions"

export async function handleClickReprocessarDocumento(context) {
  const { dataTables, datas } = context

  const result = await reprocessarDocumento(datas.documentoId)

  if (result.isFailure()) return

  closeModal({ id: 'listaClausulasModal' })

  dataTables.documentosTb.reload()
}
