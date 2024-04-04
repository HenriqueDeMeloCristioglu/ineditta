export function stringSelect({
  className = '',
  children = '',
  configs = '',
  id = ''
}) {
  return `<select class="` + className + `" ` + className + `" ` + configs + ` id="` + id + `">` + children + `</select>`
}