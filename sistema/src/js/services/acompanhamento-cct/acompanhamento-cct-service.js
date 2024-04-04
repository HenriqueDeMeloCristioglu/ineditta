import { MediaType } from '../../utils/web'
import DateParser from "../../utils/date/date-parser"

export class AcompanhamentoCctService {
  constructor(apiService, apiLegadoService) {
    this.apiService = apiService
    this.apiLegadoService = apiLegadoService
    this.url = "v1/acompanhamentos-cct"
  }

  async obterTodos() {
    return await this.apiService.get(this.url);
  }

  async salvar(acompanhamento) {
    if (acompanhamento && acompanhamento['dataInicial'] instanceof Date) {
      acompanhamento['dataInicial'] = DateParser.toString(acompanhamento['dataInicial']);
    }

    if (acompanhamento && acompanhamento['dataFinal'] instanceof Date) {
      acompanhamento['dataFinal'] = DateParser.toString(acompanhamento['dataFinal']);
    }

    if (acompanhamento && acompanhamento['dataProcessamento'] instanceof Date) {
      acompanhamento['dataProcessamento'] = DateParser.toString(acompanhamento['dataProcessamento']);
    }

    if (acompanhamento && acompanhamento['validadeFinal'] instanceof Date) {
      acompanhamento['validadeFinal'] = DateParser.toString(acompanhamento['validadeFinal']);
    }

    return await this.apiService.post(`${this.url}`, acompanhamento, MediaType.json);
  }

  async editar(acompanhamento) {
    if (acompanhamento && acompanhamento['dataInicial'] instanceof Date) {
      acompanhamento['dataInicial'] = DateParser.toString(acompanhamento['dataInicial']);
    }

    if (acompanhamento && acompanhamento['dataFinal'] instanceof Date) {
      acompanhamento['dataFinal'] = DateParser.toString(acompanhamento['dataFinal']);
    }

    if (acompanhamento && acompanhamento['dataProcessamento'] instanceof Date) {
      acompanhamento['dataProcessamento'] = DateParser.toString(acompanhamento['dataProcessamento']);
    }

    if (acompanhamento && acompanhamento['validadeFinal'] instanceof Date) {
      acompanhamento['validadeFinal'] = DateParser.toString(acompanhamento['validadeFinal']);
    }

    return await this.apiService.put(`${this.url}/${acompanhamento.id}`, acompanhamento, MediaType.json);
  }

  async postLegado(data) {
    return await this.apiLegadoService.post("includes/php/ajax.php", data, MediaType.urlencoded);
  }

  async adionarScript(data) {
    return await this.apiService.patch(`${this.url}/scripts`, data, MediaType.json)
  }

  async enviarEmailContato(data) {
    return await this.apiService.post(`${this.url}/email-contato`, data, {
      'Content-Type': 'application/json',
      'X-request-ID': data.idempotentToken
    });
  }

  async excluir(id) {
    return await this.apiService.delete(`${this.url}/${id}`);
  }

  async obterPorId(id) {
    return await this.apiService.get(`${this.url}/${id}`);
  }

  async obterUsuariosPorGruposId({ id, params }) {
    return await this.apiService.get(`${this.url}/usuarios-grupos/${id}`, params, MediaType.json);
  }

  async obterDatatable(params) {
    return await this.apiService.get(`${this.url}`, params, MediaType.dataTable);
  }

  async obterRelatorioDatatable(params) {
    if (params && params['dataProcessamento'] instanceof Date) {
      params['dataProcessamento'] = DateParser.toString(params['dataProcessamento']);
    }
    
    if (params && params['dataProcessamentoInicial'] instanceof Date) {
      params['dataProcessamentoInicial'] = DateParser.toString(params['dataProcessamentoInicial']);
    }
    
    if (params && params['dataProcessamentoFinal'] instanceof Date) {
      params['dataProcessamentoFinal'] = DateParser.toString(params['dataProcessamentoFinal']);
    }

    return await this.apiService.get(`${this.url}/datatable-relatorio`, params, MediaType.dataTable);
  }

  async obterInformacoesIniciais() {
    return await this.apiService.get(`${this.url}/informacoes-inicias`);
  }

  async obterSelectTiposDocs(params) {
    return await this.apiService.get(`${this.url}/tipos-docs`, params, MediaType.select);
  }

  async obterFasesFiltro() {
    return await this.apiService.get(`${this.url}/fases-filtro`, null, MediaType.select);
  }

  async obterAnoBaseFiltro() {
    return await this.apiService.get(`${this.url}/ano-base-filtro`, null, MediaType.select)
  }

  async obterNomeDocumentoFiltro() {
    return await this.apiService.get(`${this.url}/nome-documento-filtro`, null, MediaType.select);
  }

  async obterFuturasDatatable(params) {
    return await this.apiService.get(`${this.url}/futuras`, params, MediaType.dataTable);
  }

  async obterNegociacoesFases(params) {
    return await this.apiService.get(`${this.url}/negociacoes-fases`, params, MediaType.json);
  }

  async obterNegociacoesFasesUfs(params) {
    return await this.apiService.get(`${this.url}/negociacoes-fases-ufs`, params, MediaType.json);
  }

  async obterNegociacoesAbertas(params) {
    return await this.apiService.get(`${this.url}/negociacoes-abertas`, params, MediaType.json);
  }

  async obterPerguntasRespostasFases(id) {
    return await this.apiService.get(`${this.url}/${id}/fases/respostas`, null, MediaType.json);
  }

  async downloadRelatorioNegociacoes(params) {
    if (params && params['dataProcessamento'] instanceof Date) {
      params['dataProcessamento'] = DateParser.toString(params['dataProcessamento']);
    }
    
    if (params && params['dataProcessamentoInicial'] instanceof Date) {
      params['dataProcessamentoInicial'] = DateParser.toString(params['dataProcessamentoInicial']);
    }
    
    if (params && params['dataProcessamentoFinal'] instanceof Date) {
      params['dataProcessamentoFinal'] = DateParser.toString(params['dataProcessamentoFinal']);
    }

    return await this.apiService.download(`${this.url}/relatorios-negociacoes`, params, MediaType.stream)
  }

  async obterSelectDataBase() {
    return await this.apiService.get(`${this.url}/datas-bases`, null, MediaType.select)
  }

  async obterSelectLocalidades(params) {
    return await this.apiService.get(`${this.url}/localidades`, params, MediaType.select);
  }
}
