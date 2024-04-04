import { stringInput } from "../../string-elements"

export function createInfoAdicionalInput({
  type = 'text',
  id,
  className = '',
  placeholder = ''
}) {
  return stringInput({
    type,
    id,
    className: `form-control info-adicional ${className}`,
    placeholder,
    style: 'width: 200px;'
  })
}

