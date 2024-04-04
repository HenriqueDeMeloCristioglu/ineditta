const { UseCaseAction } = require('../cli-actions')

class CliActionMediator {
  constructor() {
    this.useCaseAction = new UseCaseAction()
  }

  send(options) {
    if (options.action) {
      this.useCaseAction.criar(options.module, options.action, options.submodule)
    }
  }
}

module.exports = {
  CliActionMediator
}
