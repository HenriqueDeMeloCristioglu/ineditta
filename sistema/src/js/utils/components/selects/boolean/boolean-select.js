import { BooleanType } from './boolean-constant'

export async function obterBooleanSelect() {
  const data = [{
    id: `${BooleanType.Nao}`,
    description: 'Não'
  }, {
    id: `${BooleanType.Sim}`,
    description: 'Sim'
  }]

  return await Promise.resolve(data)
}