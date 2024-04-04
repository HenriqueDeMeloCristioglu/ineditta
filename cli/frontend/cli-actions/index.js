const { AtualizarAction } = require('./crud/atualizar-action')
const { BaseCrudAction } = require('./crud/base-crud-action')
const { IncluirAction } = require('./crud/incluir-action')
const { ObterPorIdAction } = require('./crud/obter-por-id-action')
const { ServiceAction } = require('./service-action')

module.exports = {
  IncluirAction,
  BaseCrudAction,
  AtualizarAction,
  ObterPorIdAction,
  ServiceAction
}