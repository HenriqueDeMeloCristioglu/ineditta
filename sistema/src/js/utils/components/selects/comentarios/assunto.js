import { TipoComentario } from "../../../../application/comentarios/constants"

// Core
import { ApiService } from "../../../../core/index"
import { ApiLegadoService } from "../../../../core/api-legado"

// Service
import {
  ClausulaGeralService,
  ClienteUnidadeService,
  SindicatoLaboralService,
  SindicatoPatronalService
} from "../../../../services"

const apiService = new ApiService()
const apiLegadoService = new ApiLegadoService()
const clausulaGeralService = new ClausulaGeralService(apiService, apiLegadoService)
const sindicatoPatronalService = new SindicatoPatronalService(apiService, apiLegadoService)
const sindicatoLaboralService = new SindicatoLaboralService(apiService, apiLegadoService)
const clienteUnidadeService = new ClienteUnidadeService(apiService)

export async function obterAssunto(tipoComentarioId) {
  tipoComentarioId = parseFloat(tipoComentarioId)

  switch (tipoComentarioId) {
    case TipoComentario.Clausula:
      return (await clausulaGeralService.obterSelect()).value
    case TipoComentario.SindicatoPatronal:
      return await sindicatoPatronalService.obterSelectPorUsuario()
    case TipoComentario.SindicatoLaboral:
      return await sindicatoLaboralService.obterSelectPorUsuario()
    case TipoComentario.Filial:
      return (await clienteUnidadeService.obterSelect()).value
    default:
      return await Promise.resolve([])
  }
}