import { stringSelect } from "../../string-elements";

export function createInfoAdicionalSelect({
  placeholder = '',
  id
}) {
  return stringSelect({
    id,
    className: `form-control info-adicional select2 chzn-select combo`,
    configs: `data-placeholder="` + placeholder + `"`
  })
}