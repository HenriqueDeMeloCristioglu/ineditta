import { TipoComentario } from "../../../../application/comentarios/constants"


export async function obterTipoComentarioSelect({
  showFilial
}) {
  const data = [{
    id: '',
    description: '--'
  }, {
    id: TipoComentario.Clausula,
    description: 'Cláusula'
  }, {
    id: TipoComentario.SindicatoPatronal,
    description: 'Sindicato Patronal'
  }, {
    id: TipoComentario.SindicatoLaboral,
    description: 'Sindicato Laboral'
  }]

  if (showFilial) {
    data.push({
      id: 'filial',
      description: 'Estabelecimento'
    })
  }

  return await Promise.resolve(data)
}