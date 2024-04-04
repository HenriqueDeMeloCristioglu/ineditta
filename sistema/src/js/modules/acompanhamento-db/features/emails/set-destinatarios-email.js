import $ from 'jquery'
import { ArrayUtils } from '../../../../utils/util'

export function setDestinatariosEmail(dest) {
  const currentValue = $("#para-input").val()

  let values = ArrayUtils.removeEmptyStrings(currentValue.split(','))
  values = ArrayUtils.trim(values)

  let destinations = ArrayUtils.removeEmptyStrings(dest.split(','))
  destinations = ArrayUtils.trim(destinations)

  if (!ArrayUtils.includesArray(values, destinations)) {
    destinations.map(destination => values.push(destination))
  }

  $("#para-input").val(values.join(', '))
}