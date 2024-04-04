export function stringTbody({ className = '', id = '', children = '' }) {
  return `<tbody class="` + className + `" id="` + id + `">` + children + `</tbody>`
}
