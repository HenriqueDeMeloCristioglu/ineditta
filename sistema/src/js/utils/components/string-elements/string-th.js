export function stringTh({ className = '', id = '', content = '' }) {
  return `<th class="` + className + `" id="` + id + `">` + content + `</th>`
}
