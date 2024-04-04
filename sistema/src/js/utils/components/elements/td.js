import $ from 'jquery'

export function td({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<td>', {
    id,
    class: className,
    style
  }).html(content)
}