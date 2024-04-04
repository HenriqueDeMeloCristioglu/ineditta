export function stringTd({ className = '', id = '', content = '', style = '' }) {
  return `<td class="` + className + `" id="` + id + `" style="` + style + `">` + content + `</td>`
}
