import $ from 'jquery'
import { obterClausulaPorId } from '../../../obter-dados'
import { GrupoInformacoesAdicionais } from '../../../../components'

export async function handleOpenModalUpsertClausula(context) {
  const { datas, inputs: { selects, datePickers } } = context

  selects.documentoSindicalSelect.setCurrentValue({
    id: datas.documentoSelecionado.id,
    description: datas.documentoSelecionado.nome
  })

  datePickers.dataInicialDt.setValue(datas.documentoSelecionado.vigenciaInicial)
  datePickers.dataFinalDt.setValue(datas.documentoSelecionado.vigenciaFinal)

  if (!datas.clausulaId) return

  datas.clausulaSelecionada = await obterClausulaPorId(datas.clausulaId)

   selects.estruturaClausulaSelect.setCurrentValue({
    id: datas.clausulaSelecionada.estruturaClausulaId,
    description: datas.clausulaSelecionada.estruturaClausulaNome
  })

  selects.sinonimoSelect.setCurrentValue({
    id: datas.clausulaSelecionada.sinonimoId,
    description: datas.clausulaSelecionada.sinonimoNome
  })
  
  $("#numero").val(datas.clausulaSelecionada.numero)
  $("#texto").val(datas.clausulaSelecionada.texto)

  const grupoInformacaoAdicional = datas.clausulaSelecionada.grupoInformacaoAdicional
  if (grupoInformacaoAdicional) {
    const grupoInformacoesAdicionais = new GrupoInformacoesAdicionais([grupoInformacaoAdicional])
    grupoInformacoesAdicionais.create(context)
    grupoInformacoesAdicionais.select()
  }
}