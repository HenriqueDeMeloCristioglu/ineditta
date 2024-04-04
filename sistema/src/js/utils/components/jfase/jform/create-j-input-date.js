import { stringInput } from "../../string-elements"

export function createJInputDate({ id }) {
  return stringInput({
    id,
    className: 'form-control',
    placeholder: 'DD/MM/AAAA',
    type: 'date'
  })
}
