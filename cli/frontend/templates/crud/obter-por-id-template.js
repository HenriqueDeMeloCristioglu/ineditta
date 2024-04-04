const { BaseCrudTemplate } = require("./base-crud-template")

class ObterPorIdTemplate {
  static create(moduleName) {
    return BaseCrudTemplate.create(moduleName, 'obterPorId')
  }
}

module.exports = {
  ObterPorIdTemplate
}