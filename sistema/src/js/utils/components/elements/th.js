import $ from 'jquery'

export function th({
  id = '',
  className = '',
  content = '',
  style = ''
}) {
  return $('<th>', {
    id,
    class: className,
    styleSheets: style
  }).html(content)
}