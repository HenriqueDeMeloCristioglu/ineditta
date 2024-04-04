import { handleOpenModalListaClausula } from "../modals/lista-clausulas/handle-open-modal-lista-clausula"

export async function handleClickOrdenarPorStatusConsistente(context) {
  const { datas } = context
  datas.ordernarPorStatusConsistente = !datas.ordernarPorStatusConsistente

  await handleOpenModalListaClausula(context)
}