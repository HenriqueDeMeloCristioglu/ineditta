import { ApiService } from "../../../../../../core/index"
import { ClausulaClienteService } from "../../../../../../services"
import { useClausulaGeralService } from "../../../../../core/hooks"
import $ from 'jquery'

const clausulaGeralService = useClausulaGeralService()
const clausulaClienteService = new ClausulaClienteService(new ApiService())

export async function handleOpenClausulaClienteModal(context) {
  const { datas } = context

  const clausulaId = datas.clausulaId

  const reultClausulaGeral = await clausulaGeralService.obterPorId(clausulaId)
  if (reultClausulaGeral.isFailure()) return

  $('#nome_clausula_cliente').val(reultClausulaGeral.value.nome)

  const reultClausulaCliente = await clausulaClienteService.obterPorId(clausulaId)
  if (reultClausulaCliente.isFailure()) return

  datas.clausulaCliente = reultClausulaCliente.value

  $('#texto_clausula_cliente').val(datas.clausulaCliente.texto)
}
