import $ from 'jquery'

export function button({
  id = '',
  type = 'submit',
  className = '',
  style = '',
  title = '',
  content = ''
}) {
  return $('<button>', {
    id,
    type,
    class: className,
    title,
    style
  }).html(content)
}