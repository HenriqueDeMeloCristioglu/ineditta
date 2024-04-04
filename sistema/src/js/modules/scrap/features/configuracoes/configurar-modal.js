import { ModalListaClausula, ModalUpsertClausula } from "../../components"

export function configurarModal(context) {
  const pageCtn = document.getElementById('pageCtn')

  const modalListaClausula = new ModalListaClausula({ pageContainer: pageCtn })
  modalListaClausula.render(context)

  const modalUpsertClausula = new ModalUpsertClausula({ pageContainer: pageCtn })
  modalUpsertClausula.render(context)
}
