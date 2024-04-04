import { obterUrlDocumentoPorDocumentoSindicalId } from "../../../../../../application/documento-sindical/features/obter-dados/obter-url-documento"
import { criarClausulaItem } from "../../../lista-clausulas-item"
import { obterDocumentoPorId } from "../../../obter-dados"
import { handleClickRemovarClausula } from "../../clausulas"
import { handleClickEditarClausula } from "../../clausulas/handle-click-editar-clausula"
import { handleClickOrdenarPorStatusConsistente } from "../../clausulas/handle-click-ordenar-por-status-consistente"
import $ from 'jquery'

export async function handleOpenModalListaClausula(context) {  
  const { datas } = context

  datas.documentoSelecionado = await obterDocumentoPorId(datas.documentoId)
  context.datas.documentoPath = await obterUrlDocumentoPorDocumentoSindicalId(datas.documentoReferenciaId)

  if (datas.ordernarPorStatusConsistente) {
    datas.documentoSelecionado?.clausulas.sort((clausulaA, clausulaB) => clausulaB.status - clausulaA.status)
  }

  $("#lista_clausulas").html('')

  datas.documentoSelecionado?.clausulas.map(clausula => {
    const clausulaItem = criarClausulaItem({
      id: clausula.id,
      assunto: clausula.assunto,
      classificacaoClausula: clausula.estruturaClausulaNome,
      numero: clausula.numero,
      sinonimo: clausula.sinonimoNome,
      status: clausula.status,
      texto: clausula.texto
    },
    handleClickEditarClausula,
    handleClickRemovarClausula,
    handleClickOrdenarPorStatusConsistente,
    context)

    $("#lista_clausulas").append(clausulaItem)
  })
}
