import $ from 'jquery'

export function h4({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<h4>', {
    id,
    class: className,
    style
  }).html(content)
}