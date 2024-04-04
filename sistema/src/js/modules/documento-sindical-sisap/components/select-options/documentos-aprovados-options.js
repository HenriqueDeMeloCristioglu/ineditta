import { ApiService } from "../../../../core/index"
import { DocumentosLocalizadosService } from "../../../../services"
import DateFormatter from "../../../../utils/date/date-formatter"

const documentosLocalizadosService = new DocumentosLocalizadosService(new ApiService())

export async function documentosAprovadosOptions() {
  const result = await documentosLocalizadosService.obterAprovados()

  if(result.isFailure()) return

  const data = result.value
  
  const options = []

  data.map(({ id, nome, dataAprovacao }) => {
    options.push({
      id,
      description: `${id}-${nome} / Aprovação: ${DateFormatter.dayMonthYear(dataAprovacao)}`
    })
  })

  return options
}