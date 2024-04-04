export function stringTr({ className = '', id = '', style = '', content = '' }) {
  return `<tr class="` + className + `" id="` + id + `" style="` + style + `">` + content + `</tr>`
}
