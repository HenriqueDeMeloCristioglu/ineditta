import NotificationService from "../../../../../../utils/notifications/notification.service"
import { PlaceholderCaixaDeSelecao } from "../../../../components/informacoes-adicionais/grupo-infromacoes-adicionais/components"
import $ from 'jquery'

export function handleClickRemoverInformacaoAdicionalGrupo() {
  NotificationService.warning({
    title: 'Você irá excluir a informação adicional!',
    showConfirmButton: true,
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Sim, excluir!',
    then: (result) => {
      if (!result.isConfirmed) return
    
      var caixaDeSelecao = new PlaceholderCaixaDeSelecao()
      $('#infoAdicional_grupo_lista_selecao').html(caixaDeSelecao.criar())
      $('#informacao_adicional_placeholder').show()
      
      $('.btn_selecionar_informacao_adicional_grupo').prop('disabled', false)
      
      $('#table_grupo').html('')
      $("#table-grupo-add").hide()

      NotificationService.success({ title: 'Excluído!' })
    }
  })
}