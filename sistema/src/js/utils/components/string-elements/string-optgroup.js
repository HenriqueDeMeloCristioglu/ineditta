export function stringOptgroup({
  label = '',
  options
}) {
  return `<optgroup label="` + label + `"> ` + options + ` </optgroup>`
}