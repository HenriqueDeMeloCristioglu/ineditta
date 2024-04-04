export function stringB({
  text,
  className = ''
}) {
  return `<b class="` + className + `">` + text + `</b>`
}