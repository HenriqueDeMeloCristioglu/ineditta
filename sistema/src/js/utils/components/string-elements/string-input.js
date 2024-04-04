export function stringInput({
  className = '',
  type = 'text',
  id,
  placeholder = '',
  name = '',
  checked = false,
  value = '',
  style = ''
}) {
  return `<input
    type="` + type + `"
    name="` + name + `"
    checked="` + checked + `"
    class="` + className + `"
    id="` + id + `"
    placeholder="` + placeholder + `"
    value="`+ value + `"
    style="`+ style + `"
  />`
}