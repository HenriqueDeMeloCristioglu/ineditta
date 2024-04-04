export default class JsonFormatter {
  static joinColumns(json, options = {}) {
    let separator = ', '

    if (options.separator) separator = options.separator

    const array = JSON.parse(json ?? '')

    if (array.length <= 0) return

    if (!options.column) return array.join(separator)

    let arrayItem = []
    array.map(item => {
      const itemColumn = item[options.column]

      if (!itemColumn) return

      arrayItem.push(itemColumn)
    })

    return arrayItem.join(separator)
  }
}