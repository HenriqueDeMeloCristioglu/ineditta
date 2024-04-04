import $ from 'jquery'

export function a({
  id = '',
  className = '',
  style = '',
  title = '',
  content = '',
  href = '#'
}) {
  return $('<a>', {
    id,
    class: className,
    title,
    style,
    href
  }).html(content)
}