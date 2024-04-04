import { ApiService } from "../../../../../core/index"
import { DocSindService } from "../../../../../services"

const docSindService = new DocSindService(new ApiService())

export async function obeterDocumentoPorId(id) {
  return await docSindService.obterDocSindPorId(id)
}