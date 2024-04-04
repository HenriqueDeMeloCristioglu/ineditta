import NotificationService from "../../../../utils/notifications/notification.service"

export function removerInformacaoAdicional(resetInformacaoAdicional) {
  NotificationService.warning({
    title: 'Você irá excluir a informação adicional!',
    showConfirmButton: true,
    showCancelButton: true,
    confirmButtonColor: '#3085d6',
    cancelButtonColor: '#d33',
    confirmButtonText: 'Sim, excluir!',
    then: (result) => {
      if (!result.isConfirmed) return

      resetInformacaoAdicional()

      NotificationService.success({ title: 'Excluído!' })
    }
  })
}