import $ from 'jquery'

export function h2({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<h2>', {
    id,
    class: className,
    style
  }).html(content)
}