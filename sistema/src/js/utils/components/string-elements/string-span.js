export function stringSpan({ className = '', children = '' }) {
  return `<span class="` + className + `">` + children + `</span>`
}