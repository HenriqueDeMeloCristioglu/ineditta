import { stringLabel } from "../../string-elements"

export function createJLabel({ text, id, className }) {
  return stringLabel({
    id,
    text: String(text),
    className
  })
}