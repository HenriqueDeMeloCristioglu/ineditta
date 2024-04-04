import { MediaType } from "../utils/web"

export class MatrizService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = "v1/matrizes"
  }

  async obterDataTable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterSelect(grupoId) {
    return await this.apiService.get(`${this.url}`, { grupoEconomicoId: grupoId }, MediaType.select)
  }

  async obterSelectTodos() {
    return await this.apiService.get(`${this.url}/todos`, null, MediaType.select)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json);
  }

  async obterSelectPorUsuario(grupoEconomicoId) {
    let url = `${this.url}?porUsuario=true&columns=id,nome,cnpj,codigo`
    let paramsGrupos = ""
    let gruposEconomicosIds = ""

    if (grupoEconomicoId instanceof Array) {
      paramsGrupos =
        gruposEconomicosIds != null
          ? grupoEconomicoId.map((n) => `gruposEconomicosIds=${n}`).join("&")
          : ""
      url = `${this.url}?porUsuario=true&columns=id,nome,cnpj,codigo&${paramsGrupos}`
    } else if (grupoEconomicoId) {
      url = `${this.url}?porUsuario=true&grupoEconomicoId=${grupoEconomicoId}&columns=id,nome,cnpj,codigo`
    }
    const result = await this.apiService.get(url, null, MediaType.json)
    return result.isSuccess() && Array.isArray(result.value)
      ? result.value?.map((empresa) => ({
        id: empresa.id,
        description: `Cód: ${empresa.codigo} / Nome: ${empresa.nome}`,
      })) ?? []
      : []
  }

  async inserir(params) {
    return await this.apiService.post(`${this.url}`, params, MediaType.multipartFormData);
  }

  async atualizar(params) {
    return await this.apiService.put(`${this.url}`, params, MediaType.multipartFormData);
  }

  async inativarAtivarToggle(id) {
    return await this.apiService.patch(`${this.url}/${id}`, null, MediaType.json);
  }
}
