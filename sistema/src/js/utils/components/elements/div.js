import $ from 'jquery'

export function div(params = {
  id: '',
  className: '',
  style: '',
  content: ''
}) {
  const { id, className, style, content } = params;
  return $('<div>', {
    id,
    class: className,
    style
  }).html(content)
}