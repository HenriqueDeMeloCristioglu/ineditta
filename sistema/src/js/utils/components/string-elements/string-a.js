export function stringA({
  text,
  href,
  target = '',
  className = ''
}) {
  return `<a href="` + href + `" target="` + target + `" class="` + className + `">` + text + `</a>`
}