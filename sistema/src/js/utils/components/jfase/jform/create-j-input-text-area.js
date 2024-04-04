import { stringTextArea } from "../../string-elements"

export function createJInputTextArea({ id }) {
  return stringTextArea({
    id,
    className: 'form-control'
  })
}
