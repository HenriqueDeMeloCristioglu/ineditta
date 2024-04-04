const { BaseCrudTemplate } = require("./base-crud-template")

class IncluirTemplate {
  static create(moduleName) {
    return BaseCrudTemplate.create(moduleName, 'incluir')
  }
}

module.exports = {
  IncluirTemplate
}