import { useClausulaService } from "../../../../../core/hooks"
import $ from 'jquery'
import { EstabelecimentoPainel } from "../../../../components/modal-painel/components"
import { ModalPainel } from "../../../../components"
import { handleClickAdicionarRegraEmpresaBtn } from "../../formulario"

const clausulaService = useClausulaService()

export async function handleOpenClausulaModal(context) {
  const { datas, inputs: { selects } } = context

  const ids = datas.clausulasSelecionadas

  if (!datas.clausulaClicada && ids.length <= 0) {
    return
  }

  const clausulasIds = datas.clausulaClicada ? [datas.clausulaClicada] : ids

  const result = await clausulaService.obterPorId(clausulasIds)
  
  const data = result.value
  
  const clausulasContainer = $("#clausulaModalContainer")

  data.forEach(async (data) => {
    const id = parseInt(data?.id)
    const grupoEconomicoId = parseInt(selects.gruposEconomicosSelect.getValue())
    
    const estabelecimentoPainel = new EstabelecimentoPainel({ id, unidades: data.unidade, grupoEconomicoId })
    
    const modalPainel = new ModalPainel({
      id,
      data,
      handleClickAdicionarRegraEmpresaBtn,
      modulos: datas.modulos,
      estabelecimentoPainel: estabelecimentoPainel.create(),
      grupoEconomicoId
    })
    const modalPainelWrapper = await modalPainel.create(context)

    clausulasContainer.append(modalPainelWrapper)

    estabelecimentoPainel.init()
  })
}
