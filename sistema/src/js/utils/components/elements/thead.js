import $ from 'jquery'

export function thead({
  id = '',
  className = '',
  content = ''
}) {
  return $('<thead>', {
    id,
    class: className,
  }).html(content)
}