const { AtualizarTemplate } = require('./crud/atualizar-template')
const { BaseCrudTemplate } = require('./crud/base-crud-template')
const { IncluirTemplate } = require('./crud/incluir-template')
const { ObterPorIdTemplate } = require('./crud/obter-por-id-template')
const { ServiceTemplate } = require('./service-template')

module.exports = {
  IncluirTemplate,
  BaseCrudTemplate,
  AtualizarTemplate,
  ObterPorIdTemplate,
  ServiceTemplate
}

