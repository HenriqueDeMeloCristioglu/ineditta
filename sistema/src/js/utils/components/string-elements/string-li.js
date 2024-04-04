export function stringLi({
  className = '',
  style = '',
  content = ''
}) {
  return `<li class="` + className + `" style="` + style + `">` + content + `</li>`
}
