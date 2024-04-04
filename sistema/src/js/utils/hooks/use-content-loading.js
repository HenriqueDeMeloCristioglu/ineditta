import $ from 'jquery'

export async function useContentLoading(cb) {
  $('#hide-page').show()
  await cb()
  $('#hide-page').hide()
}
