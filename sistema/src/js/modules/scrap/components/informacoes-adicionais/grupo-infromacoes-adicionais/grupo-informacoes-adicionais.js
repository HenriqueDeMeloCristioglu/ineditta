import $ from 'jquery'
import { Generator } from '../../../../../utils/generator'
import { handleClickSelecionarInformacaoAdicionalGrupo } from '../../../index'
import { InformacaoAdicionalGrupo, PlaceholderCaixaDeSelecao } from './components'

export class GrupoInformacoesAdicionais {
  constructor(data) {
    this.data = data
    this.id = Generator.id()
  }

  create(context) {
    const caixaDeSelecao = new PlaceholderCaixaDeSelecao()
    $('#infoAdicional_grupo_lista_selecao').html(caixaDeSelecao.criar())

    let infoAdicionais = this.data.map(({ nomeTipoInformacao, tipoInformacaoId }) => {
      const informacaoAdicionalGrupo = new InformacaoAdicionalGrupo()

      return informacaoAdicionalGrupo.criar({ id: tipoInformacaoId, nomeTipoInformacao })
    })
    $("#infoAdicionalGrupoSelecao").append(infoAdicionais)

    $('.btn_selecionar_informacao_adicional_grupo').on('click', async (el) => await handleClickSelecionarInformacaoAdicionalGrupo({ element: el, data: this.data, id: this.id, context }))

    $('#informacao_adicional_grupo_painel').show()
  }

  select() {
    $('.btn_selecionar_informacao_adicional_grupo').trigger('click')
  }
}