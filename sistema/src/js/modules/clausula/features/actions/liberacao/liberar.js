import { closeModal } from "../../../../../utils/modals/modal-wrapper"
import Result from "../../../../../core/result"
import NotificationService from "../../../../../utils/notifications/notification.service"
import { liberarClausulasDocumento } from "./liberar-clausulas-documento"
import { liberarDocumento } from "./liberar-documento"

export async function liberar({ dataLiberacaoClausulas, documentoSelecionadoId }) {
  return new Promise((resolve) => {
    NotificationService.success({
      title: !dataLiberacaoClausulas ? 'Tem certeza que deseja liberar o documento.' : 'Este documento já foi liberado, deseja liberar novamente?',
      showConfirmButton: true,
      showCancelButton: true,
      confirmButtonColor: '#3085d6',
      cancelButtonColor: '#d33',
      confirmButtonText: 'Sim, liberar!',
      then: async (result) => {
        if (!result.isConfirmed) {
          return resolve(Result.success())
        }

        const resultClausula = await liberarClausulasDocumento(documentoSelecionadoId)

        if (resultClausula.isFailure()) {
          return NotificationService.error({ title: 'Erro ao liberar clausulas do documento', message: resultClausula.error })
        }

        const resultDoc = await liberarDocumento(documentoSelecionadoId)

        if (resultDoc.isFailure()) {
          return NotificationService.error({ title: 'Erro ao liberar documento', message: resultDoc.error })
        }

        NotificationService.success({ title: 'Documento liberado com sucesso!' })

        closeModal({ id: 'clausulaModal' })

        return resolve(Result.success())
      },
      timer: null,
    })
  })
}