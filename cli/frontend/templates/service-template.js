const { capitalizeFirstLetter } = require("../../shared-kernel/utils")

class ServiceTemplate {
  static create(moduleName) {
    const upperModuleName = capitalizeFirstLetter(moduleName)

    return `import { MediaType } from '../utils/web'

export class ${upperModuleName}Service {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/${moduleName}'
  }

  async incluir(data) {
    return await this.apiService.post(this.url, data, MediaType.json)
  }

  async atualizar(data) {
    return await this.apiService.put(this.url, data, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(${"`${this.url}/${id}`"}, null, MediaType.json)
  }

  async obterDatatable(request) {
    return await this.apiService.get(this.url, request, MediaType.dataTable)
  }
}`
  }
}

module.exports = {
  ServiceTemplate
}