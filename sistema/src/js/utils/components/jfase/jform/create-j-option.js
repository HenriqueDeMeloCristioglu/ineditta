import { stringOption } from "../../string-elements"

export function createJOption(opcao) {
  return stringOption({
    option: opcao,
    value: opcao
  })
}
