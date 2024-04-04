import { stringDiv } from "../../string-elements"

export function createJGroup({ children, className = '' }) {
  return stringDiv({ className: `col-sm-12 ${className}`, children })
}