import $ from 'jquery'
import { deletarClausula } from '../../actions'

export async function handleClickRemovarClausula(id) {
  const result = await deletarClausula(id)

  if (result.isFailure()) return

  $(`#${id}`).remove()
}