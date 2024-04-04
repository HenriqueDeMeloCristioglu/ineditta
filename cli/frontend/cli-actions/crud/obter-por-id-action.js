const { BaseCrudAction } = require('./base-crud-action')
const { ObterPorIdTemplate } = require('../../templates')

class ObterPorIdAction {
  constructor() {
    this.baseCrudAction = new BaseCrudAction()
  }

  criar(moduleName) {
    const obterPorIdTemplate = ObterPorIdTemplate.create(moduleName)
    this.baseCrudAction.criar(moduleName, obterPorIdTemplate, 'obter-por-id')
  }
}

module.exports = {
  ObterPorIdAction
}