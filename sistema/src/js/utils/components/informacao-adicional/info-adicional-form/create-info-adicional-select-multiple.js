import { stringSelect } from "../../string-elements";

export function createInfoAdicionalSelectMultiple({
  placeholder = '',
  id
}) {
  return stringSelect({
    id,
    className: `form-control info-adicional select2 chzn-select combo-multiplo`,
    configs: `data-placeholder="` + placeholder + `" multiple`
  })
}