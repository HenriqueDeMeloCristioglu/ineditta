const { IncluirAction, AtualizarAction, ObterPorIdAction, ServiceAction } = require('../cli-actions')

class CliActionMediator {
  constructor() {
    this.incluirAction = new IncluirAction()
    this.atualizarAction = new AtualizarAction()
    this.obterPorIdAction = new ObterPorIdAction()
    this.serviceAction = new ServiceAction()
  }

  send(options) {
    if(options.crud) {
      this.incluirAction.criar(options.module)
      this.atualizarAction.criar(options.module)
      this.obterPorIdAction.criar(options.module)
    }
    
    if(options.incluir) {
      this.incluirAction.criar(options.module)
    }
    
    if(options.atualizar) {
      this.atualizarAction.criar(options.module)
    }
    
    if(options.obterPorId) {
      this.obterPorIdAction.criar(options.module)
    }

    if (options.service) {
      this.serviceAction.criar(options.module)
    }
  }
}

module.exports = {
  CliActionMediator
}
