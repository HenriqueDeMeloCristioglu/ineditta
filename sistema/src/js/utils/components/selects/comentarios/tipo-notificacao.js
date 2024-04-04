import { TipoNotificacao } from "../../../../application/comentarios/constants"

const options = [{
  id: TipoNotificacao.Fixa,
  description: 'Fixo'
}, {
  id: TipoNotificacao.Temporaria,
  description: 'Temporário'
}]

export async function obterTipoNotificacaoSelect() {
  const data = [{
    id: TipoNotificacao.Fixa,
    description: 'Fixo'
  }, {
    id: TipoNotificacao.Temporaria,
    description: 'Temporário'
  }]

  return await Promise.resolve(data)
}

export function getDescriptionTipoUsuarioNotificacao(option) {
  return options[option].description
}
