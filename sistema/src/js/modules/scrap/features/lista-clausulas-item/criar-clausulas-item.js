import { button, div, table, textarea } from "../../../../utils/components/elements"
import { criarBody } from "./criar-body"
import { criarHeader } from './index'

export function criarClausulaItem({
  id,
  classificacaoClausula,
  numero,
  sinonimo,
  status,
  texto
}, onClickEditar, onClickRemover, onClickOrdenarPorStatus, context) {
  const card = div({ id })
  const tabela = table({ className: 'clausula_tabela' })
  const thead = criarHeader({ onClickOrdenarPorStatus, context })
  const tbody = criarBody({
    classificacaoClausula,
    numero,
    sinonimo,
    status
  })

  tabela.append(thead)
  tabela.append(tbody)
  
  card.append(tabela)

  const footer = div({ className: 'clausula_ia', id: 'lista_clausulas' })
  const footerDiv = div({})
  const textoInput = textarea({ placeholder: 'Texto cláusula', content: texto }).attr('disabled', true)
  const divAcoes = div({ className: 'clausula_footer' })

  const editarBtn = button({
    type: 'button',
    id: 'editar_clausula_btn',
    className: 'btn btn-primary btn-rounded',
    content: 'Editar'
  })
  editarBtn.on('click', async () => await onClickEditar({ context, id }))

  const removerBtn = button({
    type: 'button',
    id: 'remover_documento_btn',
    className: 'btn btn-danger btn-rounded',
    content: 'Remover'
  })
  removerBtn.on('click', async () => await onClickRemover(id))

  divAcoes.append(editarBtn)
  divAcoes.append(removerBtn)
  footerDiv.append(textoInput)
  footerDiv.append(divAcoes)
  footer.append(footerDiv)

  card.append(footer)
  
  return card
}