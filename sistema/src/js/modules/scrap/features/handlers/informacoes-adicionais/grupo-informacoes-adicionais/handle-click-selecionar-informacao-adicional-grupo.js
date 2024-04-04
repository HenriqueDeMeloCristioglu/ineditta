import { InformacaoAdicionalGrupoSelecionado } from '../../../../components'
import { handleCreateListaInformacaoAdicional } from '../lista-informacoes-adicionais'
import { handleClickRemoverInformacaoAdicionalGrupo } from "./index"
import $ from 'jquery'

export async function handleClickSelecionarInformacaoAdicionalGrupo({ element, data, id, context }) {
  $('.btn_selecionar_informacao_adicional_grupo').prop('disabled', true)
  $('#informacao_adicional_placeholder').hide()
  
  const dataId = element.target.attributes['data-id'].value
  const informacaoAdicionalGrupo = data.filter(inforamcaoItem => inforamcaoItem.tipoInformacaoId == dataId)[0]

  const { nomeTipoInformacao, tipoInformacaoId, estruturaId } = informacaoAdicionalGrupo
  
  const informacaoGrupoSelecionada = new InformacaoAdicionalGrupoSelecionado()
  $('#infoAdicional_grupo_lista_selecao').append(informacaoGrupoSelecionada.criar({ id, nomeTipoInformacao }))
  
  $('.btn_remover_clausula_adicional').on('click', () => handleClickRemoverInformacaoAdicionalGrupo())

  await handleCreateListaInformacaoAdicional({
    estruturaId,
    tipoInformacaoId,
    context
  })
}