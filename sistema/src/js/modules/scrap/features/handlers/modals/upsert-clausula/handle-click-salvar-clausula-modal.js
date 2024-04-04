import { upsert } from "../../../actions"
import $ from 'jquery'
import { obterDocumentoPorId } from "../../../obter-dados"
import { criarClausulaItem } from "../../../lista-clausulas-item"
import { handleClickEditarClausula, handleClickRemovarClausula } from "../../clausulas"
import { closeModal } from "../../../../../../utils/modals/modal-wrapper"

export async function handleClickSalvarClausulaModal({ modalContainer, context }) {
  const { datas, inputs: { selects, listaInformacaoAdicional } } = context

  console.log('listaInformacaoAdicional')
  console.log(listaInformacaoAdicional)

  return

  const result = await upsert({
    id: datas.clausulaId,
    texto: $("#texto").val(),
    documentoSindicalId: datas.documentoSelecionado.id,
    estruturaClausulaId: parseInt(selects.estruturaClausulaSelect.getValue()),
    numero: $("#numero").val(),
    sinonimoId: parseInt(selects.sinonimoSelect.getValue())
  })

  $("#lista_clausulas").html('')

  datas.documentoSelecionado = await obterDocumentoPorId(datas.documentoId)

  datas.documentoSelecionado?.clausulas.map(clausula => {
    $("#lista_clausulas").append(criarClausulaItem({
      id: clausula.id,
      assunto: clausula.assunto,
      classificacaoClausula: clausula.estruturaClausulaNome,
      numero: clausula.numero,
      sinonimo: clausula.sinonimoNome,
      status: clausula.status,
      texto: clausula.texto
    }, handleClickEditarClausula, handleClickRemovarClausula, context))
  })

  if (result.isSuccess()) {
    closeModal(modalContainer)
  }
}