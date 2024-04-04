const { BaseCrudAction } = require('./base-crud-action')
const { IncluirTemplate } = require('../../templates')

class IncluirAction {
  constructor() {
    this.baseCrudAction = new BaseCrudAction()
  }

  criar(moduleName) {
    const incluirTemplate = IncluirTemplate.create(moduleName)
    this.baseCrudAction.criar(moduleName, incluirTemplate, 'incluir')
  }
}

module.exports = {
  IncluirAction
}