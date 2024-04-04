import { closeModal } from "../../../../../utils/modals/modal-wrapper"
import { aprovarDocumento } from "../../actions"

export async function handleClickAprovarDocumento(context) {
  const { dataTables, datas } = context

  const result = await aprovarDocumento(datas.documentoId)

  if (result.isFailure()) return

  closeModal({ id: 'listaClausulasModal' })

  dataTables.documentosTb.reload()
}
