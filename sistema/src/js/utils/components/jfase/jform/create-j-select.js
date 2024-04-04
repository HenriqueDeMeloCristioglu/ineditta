import { stringOptgroup, stringSelect } from "../../string-elements"

export function createJSelect({ id, options, className = '', multiple = false }) {
  return stringSelect({
    className: `form-control ${className}`,
    id,
    configs: multiple ? 'multiple' : '',
    children: stringOptgroup({
      options,
      label: 'SELECIONE'
    })
  })
}
