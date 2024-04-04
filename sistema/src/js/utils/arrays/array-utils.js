export class ArrayUtils {
  static removeEmptyStrings(array) {
    return array.filter(str => str !== "")
  }

  static trim(array) {
    return array.map(str => str.trim())
  }

  static includesArray(array, compare) {
    let conditional = false

    array.map(a => {
      compare.map(c => {
        if (a == c) conditional = true
      })
    })

    return conditional
  }

  static dateTimeSort(array, field, order = 'asc') {
    let sortable = []

    sortable = array.sort((a, b) => {
      const dataA = new Date(a[field])
      const dataB = new Date(b[field])

      if (order === 'asc') {
        if (dataA < dataB) {
          return -1
        } else if (dataA > dataB) {
          return 1
        } else {
          const horaA = a[field].getHours() * 60 + a[field].getMinutes()
          const horaB = b[field].getHours() * 60 + b[field].getMinutes()

          return horaA - horaB;
        }
      }

      if (dataA > dataB) {
        return -1
      } else if (dataA < dataB) {
        return 1
      } else {
        const horaA = a[field].getHours() * 60 + a[field].getMinutes()
        const horaB = b[field].getHours() * 60 + b[field].getMinutes()

        return horaA - horaB;
      }
    })

    return sortable
  }
}