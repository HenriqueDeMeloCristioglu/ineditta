import { TipoUsuarioDestino } from "../../../../application/comentarios/constants"

const options = [{
  id: TipoUsuarioDestino.Grupo,
  description: 'Grupo Econômico'
}, {
  id: TipoUsuarioDestino.Matriz,
  description: 'Empresa'
}, {
  id: TipoUsuarioDestino.Unidade,
  description: 'Estabelecimento'
}]

export async function obterTipoUsuarioDestinoSelect() {
  const data = [{
    id: '',
    description: '--'
  }, 
  ...options]

  return await Promise.resolve(data)
}

export function getDescriptionTipoUsuarioDestino(option) {
  return options[option].description
}
