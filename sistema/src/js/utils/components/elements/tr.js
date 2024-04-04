import $ from 'jquery'

export function tr({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<tr>', {
    id,
    class: className,
    style
  }).html(content)
}