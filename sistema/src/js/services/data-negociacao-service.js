export class DataNegociacaoService {
    _dataNegociacao = [
        { id: 1, nome: 'JAN' },
        { id: 2, nome: 'FEV' },
        { id: 3, nome: 'MAR' },
        { id: 4, nome: 'ABR' },
        { id: 5, nome: 'MAI' },
        { id: 6, nome: 'JUN' },
        { id: 7, nome: 'JUL' },
        { id: 8, nome: 'AGO' },
        { id: 9, nome: 'SET' },
        { id: 10, nome: 'OUT' },
        { id: 11, nome: 'NOV' },
        { id: 12, nome: 'DEZ' }
    ]

    obterSelect() {
        return this._dataNegociacao.map(dataNegociacao => {
            return { id: dataNegociacao.id, description: dataNegociacao.nome }
        })
    }

    converterNumeroDataNegociacao(numero) {
        const result = this._dataNegociacao.filter(d => d.id == numero)
        return result.length > 0 ? result[0].nome : null
    }

    converterDataNegociacaoNumero(dataNegociacao) {
        const result = this._dataNegociacao.filter(d => d.nome == dataNegociacao)
        return result.length > 0 ? result[0].id : null
    }
}