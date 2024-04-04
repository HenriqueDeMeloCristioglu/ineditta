export const iaDocumentoSindicalContext = {
  datas: {
    documentoPath: null,
    verDocumento: false,
    clausulaId: null,
    documentoId: null,
    documentoReferenciaId: null,
    clausulaSelecionada: null,
    ordernarPorStatusConsistente: false,
    documentoSelecionado: null,
    permissoesDocumentoSindicalIa: null
  },
  dataTables: {
    documentosTb: null
  },
  inputs: {
    listaInformacaoAdicional: null,
    selects: {
      documentoSindicalSelect: null,
      estruturaClausulaSelect: null,
      sinonimoSelect: null,
      grupoEconomicoFiltroSelect: null,
      empresaFiltroSelect: null,
      grupoOperacaoFiltroSelect: null,
      atividadeEconomicaFiltroSelect: null,
      documentoFiltroSelect: null,
      nomeDocumentoFiltroSelect: null,
      statusDocumentoFiltroSelect: null,
      statusClausulasFiltroSelect: null
    },
    datePickers: {
      dataInicialDt: null,
      dataFinalDt: null,
      dataSlaDt: null
    }
  }
}