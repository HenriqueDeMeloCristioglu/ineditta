import $ from 'jquery'

export function input({
  id = '',
  type = 'text',
  className = '',
  style = '',
  content = '',
  placeholder = ''
}) {
  return $('<input>', {
    id,
    type,
    class: className,
    style,
    placeholder
  }).val(content)
}
