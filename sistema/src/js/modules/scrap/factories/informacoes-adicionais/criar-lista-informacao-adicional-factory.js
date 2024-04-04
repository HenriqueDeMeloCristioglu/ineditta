import $ from 'jquery'
import { ListaInformacaoAdicional } from "../../components"

export class CriarListaInformacaoAdicionalFactory {
  constructor(data) {
    this.data = data
    this.listaInformacaoAdicional = null
  }

  create() {
    this.listaInformacaoAdicional = new ListaInformacaoAdicional(this.data)

    $('#table_grupo').append(this.listaInformacaoAdicional.create())
  
    this.listaInformacaoAdicional.init()
  }

  adicionarLinha() {
    const linhaNova = this.listaInformacaoAdicional.addLinha()

    $('#table_grupo').append(linhaNova)

    this.listaInformacaoAdicional.init(this.listaInformacaoAdicional.linhas.length -1)
  }

  removerLinha() {
    this.listaInformacaoAdicional.removerLinha()
  }
}
