import DateParser from '../utils/date/date-parser.js'
import { MediaType } from '../utils/web'
import Masker from '../utils/masks/masker.js'

export class ClienteUnidadeService {
  constructor(apiService) {
    this.apiService = apiService
    this.url = 'v1/clientes-unidades'
    this.urlLegado = 'includes/php/ajax.php'
  }

  async incluir(clienteUnidade) {
    if (clienteUnidade && clienteUnidade['dataAusencia'] instanceof Date) {
      clienteUnidade['dataAusencia'] = DateParser.toString(clienteUnidade['dataAusencia'])
    }

    return await this.apiService.post(this.url, clienteUnidade, MediaType.json)
  }

  async editar(clienteUnidae) {
    if (clienteUnidae && clienteUnidae['dataAusencia'] instanceof Date) {
      clienteUnidae['dataAusencia'] = DateParser.toString(clienteUnidae['dataAusencia'])
    }

    return await this.apiService.put(`${this.url}/${clienteUnidae.id}`, clienteUnidae, MediaType.json)
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable)
  }

  async obterDatatablePorGrupoEconomico(params) {
    return await this.apiService.get(`${this.url}/grupo-economico`, params, MediaType.dataTable)
  }

  obterSelect() {
    return this.apiService.get(`${this.url}`, null, MediaType.select)
  }

  obterEstabelecimentosSelect(params) {
    return this.apiService.get(`${this.url}/estabelecimentos`, params, MediaType.select)
  }

  obterEstabelecimentosPorEmpresa(empresasIds) {
    const params = empresasIds != null ? empresasIds.map(n => `empresasIds=${n}`).join('&') : ''
    return this.apiService.get(`${this.url}/estabelecimentos-por-empresas?${params}`, null, MediaType.select)
  }

  async obterSelectPorUsuario(matrizesIds = 0, gruposEconomicosIds = []) {
    let result
    let paramsGrupos = ''
    let paramsMatrizes = ''

    if (gruposEconomicosIds.length > 0) {
      paramsGrupos = gruposEconomicosIds != null ? gruposEconomicosIds.map(n => `gruposEconomicosIds=${n}`).join('&') : ''
    }
    if (matrizesIds instanceof Array) {
      paramsMatrizes = matrizesIds != null ? matrizesIds.map(n => `matrizesIds=${n}`).join('&') : ''
      result = await this.apiService.get(`${this.url}?porUsuario=true&columns=id,nome,cnpj,codigo,codigoSindical&${paramsMatrizes}&${paramsGrupos}`, null)
    } else {
      result = await this.apiService.get(`${this.url}?porUsuario=true${(matrizesIds ? "&matrizId=" + matrizesIds : '')}&columns=id,nome,cnpj,codigo,codigoSindical&${paramsGrupos}`, null, MediaType.json)
    }
    return result.isSuccess() && Array.isArray(result.value) ? result.value?.map(estabelecimento => ({ id: estabelecimento.id, description: `Cód: ${estabelecimento.codigo ?? ''} / CNPJ: ${Masker.CNPJ(estabelecimento.cnpj)} / Nome: ${estabelecimento.nome} / Cod. Sind. Cliente: ${estabelecimento.codigoSindical ?? ''}` })) ?? [] : []
  }

  async obterContadores(params) {
    const request = {
      gruposEconomicosIds: params?.gruposEconomicosIds,
      matrizesIds: params?.matrizesIds,
      unidadesIds: params?.unidadesIds,
      cnaesIds: params?.cnaesIds
    }

    return await this.apiService.get(`${this.url}/contadores`, request, MediaType.json)
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`, null, MediaType.json)
  }

  async obterInfoConsultor() {
    return await this.apiService.get(`${this.url}/info-consultor`, null, MediaType.json)
  }

  async obterDatatablePorListaIds(params) {
    return await this.apiService.post(`${this.url}/obter-por-lista-ids`, params, MediaType.dataTable)
  }

  async obterEstabelecimentosSelecionados(params) {
    return await this.apiService.post(`${this.url}/estabelecimentos-selecionados`, params, MediaType.json)
  }

  async obterInformacoesEstabelecimentosDatatable(params) {
    return await this.apiService.get(`${this.url}/informacoes-estabelecimentos`, params, MediaType.dataTable)
  } 
}
