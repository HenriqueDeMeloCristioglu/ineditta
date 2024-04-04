export function stringLabel({
  text,
  id,
  className = ''
}) {
  return `<label for="` + id + `" class="` + className + `">` + text + `:</label>`
}