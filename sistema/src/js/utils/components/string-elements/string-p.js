export function stringP({
  text,
  className = ''
}) {
  return `<p class="` + className + `">` + text + `</p>`
}