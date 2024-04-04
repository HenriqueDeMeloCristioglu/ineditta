import $ from 'jquery'

export function p({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<p>', {
    id,
    class: className,
    style
  }).html(content)
}