import $ from 'jquery'

export function tbody({
  id = '',
  className = '',
  content = ''
}) {
  return $('<tbody>', {
    id,
    class: className,
  }).html(content)
}