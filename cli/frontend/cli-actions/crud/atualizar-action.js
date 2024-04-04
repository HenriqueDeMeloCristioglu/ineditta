const { BaseCrudAction } = require('./base-crud-action')
const { AtualizarTemplate } = require('../../templates')

class AtualizarAction {
  constructor() {
    this.baseCrudAction = new BaseCrudAction()
  }

  criar(moduleName) {
    const atualizarTemplate = AtualizarTemplate.create(moduleName)
    this.baseCrudAction.criar(moduleName, atualizarTemplate, 'atualizar')
  }
}

module.exports = {
  AtualizarAction
}