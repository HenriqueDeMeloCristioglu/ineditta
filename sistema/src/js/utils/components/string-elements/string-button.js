export function stringButton({
  content,
  id = '',
  className = '',
  config = '',
  style = '',
  type = 'button',
}) {
  return `<button
    id="` + id + `"
    type="` + type + `"
    class="` + className + `"
    style="` + style + `"
    ` + config + `
  ">` + content + `</button>`
}