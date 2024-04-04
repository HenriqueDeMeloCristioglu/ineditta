import { stringInput } from "../../string-elements"

export function createJInputText({ id }) {
  return stringInput({
    id,
    className: 'form-control'
  })
}