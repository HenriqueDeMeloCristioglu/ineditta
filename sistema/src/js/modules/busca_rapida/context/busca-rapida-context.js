export let buscaRapidaContext = {
  datas: {
    usuario: null,
    clausulaId: 0,
    clausulaCliente: null,
    modulos: null,
    clausulasSelecionadas: [],
    clausulaClicada: null,
    isIneditta: false,
    isGrupoEconomico: false,
    isEstabelecimento: null,
    isEmpresa: false,
    isCliente: false,
    idDoc: null,
    dadosPessoais: null
  },
  inputs: {
    datePickers: {
      processamentoDp: null  
    },
    selects: {
      gruposEconomicosSelect: null,
      empresasSelect: null,
      estabelecimentosSelect: null,
      localizacoesSelect: null,
      cnaeSelect: null,
      sindLaboralSelect: null,
      sindPatronalSelect: null,
      tipoDocSelect: null,
      grupoClausulaSelect: null,
      estruturaClausulaSelect: null,
      dataBaseSelect: null
    }
  },
  dataTables: {
    clausulasTb: null,
    diretoriaInfoSindTb: null
  }
}