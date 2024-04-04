export function stringI({
  className = '',
  style = '',
  config = ''
}) {
  return `<i style="` + style + `" class="` + className + `" ` + config + `></i>`
}