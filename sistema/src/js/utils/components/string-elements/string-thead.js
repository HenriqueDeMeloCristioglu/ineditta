export function stringThead({ className = '', id = '', children = '' }) {
  return `<thead class="` + className + `" id="` + id + `">` + children + `</thead>`
}
