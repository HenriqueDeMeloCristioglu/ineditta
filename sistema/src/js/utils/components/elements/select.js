import $ from 'jquery'

export function select({
  id = '',
  className = '',
  style = '',
  content = '',
  placeholder = '',
  multiple = false
}) {
  return $('<select>', {
    id,
    class: className,
    style
  })
    .html(content)
    .attr('placeholder', placeholder)
    .attr('multiple', multiple)
}
