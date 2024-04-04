const { program } = require('commander')

class Commander {
  options

  constructor() {
    program
      .option('-m, --module <value>', 'Nome do módulo')
      .option('-sm, --submodule <value>', 'Nome do sub módulo')
      .option('-ac, --action <value>', 'Nome do ação módulo')
      .option('-cd, --crud', 'Selecionar todas as opções de crud')
      .option('-ic, --incluir', 'Selecionar somente o incluir do crud')
      .option('-at, --atualizar', 'Selecionar somente o atualizar do crud')
      .option('-ob, --obterPorId', 'Selecionar somente o obter por id do crud')
      .option('-sv, --service', 'Selecionar geração da service')
      .parse(process.argv)

      this.options = program.opts()
  }

  getOprions() {
    return this.options
  }
}

module.exports = {
  Commander
}
