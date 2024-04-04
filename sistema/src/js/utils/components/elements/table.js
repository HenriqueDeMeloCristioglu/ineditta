import $ from 'jquery'

export function table({
  id = '',
  className = '',
  style = '',
  content = ''
}) {
  return $('<table>', {
    id,
    class: className,
    style
  }).html(content)
}