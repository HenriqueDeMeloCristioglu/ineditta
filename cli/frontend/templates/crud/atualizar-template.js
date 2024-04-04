const { BaseCrudTemplate } = require("./base-crud-template")

class AtualizarTemplate {
  static create(moduleName) {
    return BaseCrudTemplate.create(moduleName, 'atualizar')
  }
}

module.exports = {
  AtualizarTemplate
}