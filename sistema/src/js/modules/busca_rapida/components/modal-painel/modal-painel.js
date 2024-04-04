import { ClausulaClientePainel, ClausulaClienteDataTablePainel } from "./components"
import { ClausulaPainel, TextoClausulaPainel } from "./components"

export class ModalPainel {
  constructor({
    id,
    data,
    modulos,
    handleClickAdicionarRegraEmpresaBtn,
    estabelecimentoPainel,
    grupoEconomicoId
  }) {
    this.id = id
    this.data = data
    this.modulo = modulos,
    this.handleClickAdicionarRegraEmpresaBtn = handleClickAdicionarRegraEmpresaBtn,
    this.estabelecimentoPainel = estabelecimentoPainel,
    this.grupoEconomicoId = grupoEconomicoId
  }

  async create(context) {
    const modulos = this.modulo

    const clausulaPainel = new ClausulaPainel(this.id, this.data, this.estabelecimentoPainel)
    const clausulaPainelWrapper = clausulaPainel.create()

    const clausulaClienteDataTablePainel = new ClausulaClienteDataTablePainel({ grupoEconomicoId: this.grupoEconomicoId, id: this.id, data: this.data })
    const clausulaClienteDataTablePainelWrapper = await clausulaClienteDataTablePainel.create()
  
    const showAdicionarRegrasEmpresaBtn = modulos.length > 0 ? modulos.find((modulo) => modulo.modulos === "Mapa sindical – Busca rápida").comentar == "1" : false

    const textoClausulaPainel = new TextoClausulaPainel({
      data: this.data,
      id: this.id,
      handleClickAdicionarRegraEmpresaBtn: this.handleClickAdicionarRegraEmpresaBtn,
      showAdicionarRegrasEmpresaBtn
    })
    const textoClausulaPainelWrapper = textoClausulaPainel.create(context)

    const clausulaClientePainel = new ClausulaClientePainel({ data: this.data, id: this.id })
    const clausulaClienteWrapper = clausulaClientePainel.create()
    return [clausulaPainelWrapper, textoClausulaPainelWrapper, this.data.textoResumidoCliente && clausulaClienteWrapper, clausulaClienteDataTablePainelWrapper]
  }
}
