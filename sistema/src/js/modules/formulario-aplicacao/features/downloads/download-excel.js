import { UsuarioNivel } from "../../../../application/usuarios/constants/usuario-nivel"
import { ApiService } from "../../../../core/index"
import { InformacaoAdicionalClienteService } from "../../../../services"
import NotificationService from "../../../../utils/notifications/notification.service"
import PageWrapper from "../../../../utils/pages/page-wrapper"

const apiService = new ApiService()
const informacaoAdicionalClienteService = new InformacaoAdicionalClienteService(apiService)

export async function downloadExcel({ id, aprovado, nivel }) {
  if (!aprovado && nivel != UsuarioNivel.Ineditta) {
    return NotificationService.error({ title: 'É necessário aprovar o formulário para gerar o Excel' })
  }

  const result = await informacaoAdicionalClienteService.relatorio(id)

  if (result.isFailure()) {
    return NotificationService.error({ title: 'Erro ao baixar excel', message: result.error })
  }

  const bytes = result.value.data.blob

  PageWrapper.downloadExcel(bytes, 'formulario_aplicacao.xlsx')
}