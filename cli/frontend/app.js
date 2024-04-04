const { Commander } = require('../shared-kernel/commands/commander')
const { OptionsValidator } = require("../shared-kernel/validators")
const { CliActionMediator } = require('./mediators')

const commander = new Commander()
const cliActionMediator = new CliActionMediator()

const options = commander.getOprions()

OptionsValidator.validate(options)
cliActionMediator.send(options)

console.log(`Módulo: ${options.module} criado com sucesso!`)
