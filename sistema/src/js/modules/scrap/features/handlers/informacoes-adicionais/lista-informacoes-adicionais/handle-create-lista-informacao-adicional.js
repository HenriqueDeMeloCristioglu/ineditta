import { CriarListaInformacaoAdicionalFactory } from "../../../../factories/informacoes-adicionais/criar-lista-informacao-adicional-factory"
import { obterInformacoesAdicionaisPorEstruturaTipoInformcao } from "../../../obter-dados"
import $ from 'jquery'

export async function handleCreateListaInformacaoAdicional({
  tipoInformacaoId,
  estruturaId,
  context
}) {
  const { inputs } = context

  const informacoesAdicionais = await obterInformacoesAdicionaisPorEstruturaTipoInformcao({
    tipoInformacaoId,
    estruturaId
  })

  if (informacoesAdicionais.isFailure()) {
    return informacoesAdicionais
  }

  inputs.listaInformacaoAdicional = new CriarListaInformacaoAdicionalFactory(informacoesAdicionais.value) 
  inputs.listaInformacaoAdicional.create()

  $("#adicionar_linha_lista_informacoes_adicionais_btn").on('click', () => inputs.listaInformacaoAdicional.adicionarLinha())
  $("#remover_linha_lista_informacoes_adicionais_btn").on('click', () => inputs.listaInformacaoAdicional.removerLinha())

  $("#table-grupo-add").show()
}
