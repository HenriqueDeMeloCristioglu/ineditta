class OptionsValidator {
  static validate(options) {
    if (!options.module) {
      console.log('Parâmetro --module requerido')

      process.exit(1);
    }
    if (!options.crud &&
        !options.incluir &&
        !options.atualizar &&
        !options.obterPorId &&
        !options.service &&
        !options.submodule &&
        !options.action
      ) {
      console.log('Necessário passar o que deseja gerar (-cd, -ic, -at, -ob, -sv, -sm, -ac)')

      process.exit(1);
    }
  }
}

module.exports = {
  OptionsValidator
}
