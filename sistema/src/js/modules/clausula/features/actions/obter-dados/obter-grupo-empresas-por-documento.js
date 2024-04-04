import { ApiService } from "../../../../../core/index"
import { DocSindService } from "../../../../../services"
import NotificationService from "../../../../../utils/notifications/notification.service"

const docSindService = new DocSindService(new ApiService())

export async function obterGrupoEmpresasPorDocumento(id) {
  const result = await docSindService.obterGruposEconomicosEmpresasPorId(id)

  if (result.isFailure()) return NotificationService.error({ title: 'Erro ao buscar grupos e empresas do documento', message: result.error })

  return result.value
}