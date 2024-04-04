
export function stringTextArea({
  id,
  className = '',
  style = '',
  cols = '30',
  rows = '10',
  disabled = false,
  content = ''
}) {
  return `<textarea ${disabled && "disabled"} style="${style}" class="${className}" id="${id}" cols="${cols}" rows="${rows}">${content}</textarea>`
}
