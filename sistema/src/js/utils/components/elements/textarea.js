import $ from 'jquery'

export function textarea({
  id = '',
  className = '',
  style = '',
  content = '',
  placeholder = ''
}) {
  return $('<textarea>', {
    id,
    class: className,
    style,
    placeholder
  }).val(content)
}
