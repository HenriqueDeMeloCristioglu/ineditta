import $ from 'jquery'

$.fn.extend({
  deserialize() {
    return String(this).replace(/[^a-zA-Z0-9]/g, '')
  },
  deserializeLower() {
    return String(this).replace(/[^a-zA-Z0-9]/g, '').toLocaleLowerCase()
  }
})
