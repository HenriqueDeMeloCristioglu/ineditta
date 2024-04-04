import $ from 'jquery'

export function i({
  className = '',
  style = ''
}) {
  return $('<i>', {
    class: className,
    style
  })
}