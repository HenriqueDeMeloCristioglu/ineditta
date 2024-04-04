import { stringTbody } from "../../../../../utils/components/string-elements"
import { CabecalhoListaInformacaoAdicional, LinhaListaInformacaoAdicional } from "./components"
import $ from 'jquery'

export class ListaInformacaoAdicional {
  constructor(data) {
    this.data = data
    this.linhas = []
  }

  create() {
    let rows = ''
    let grupos = []

    const index = grupos.length != 0 ? grupos.length : 1
    for (let i = 0; i < index; i++) {
      const linhaListaInformacaoAdicional = new LinhaListaInformacaoAdicional(this.data)

      rows += linhaListaInformacaoAdicional.create()

      this.linhas.push(linhaListaInformacaoAdicional)
    }

    const cabecalhoListaInformacaoAdicional = new CabecalhoListaInformacaoAdicional(this.data)
    const cabecalho = cabecalhoListaInformacaoAdicional.create()
  
    const body = stringTbody({ id: 'table_grupo_body', children: rows })

    return cabecalho + body
  }

  init(line = null) {
    if (line) {
      this.linhas[line].init()
    }

    this.linhas.map(linha => linha.init())
  }

  addLinha() {
    let rows = ''
    let grupos = []

    const index = grupos.length != 0 ? grupos.length : 1
    for (let i = 0; i < index; i++) {
      const linhaListaInformacaoAdicional = new LinhaListaInformacaoAdicional(this.data)

      rows += linhaListaInformacaoAdicional.create()

      this.linhas.push(linhaListaInformacaoAdicional)
    }

    return stringTbody({ id: 'table_grupo_body', children: rows })
  }

  removerLinha() {
    const ultimaLinha = this.linhas[this.linhas.length -1]

    $(`#${ultimaLinha.id}`).remove()

    this.linhas.pop()
  }
}