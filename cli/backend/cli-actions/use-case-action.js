const fs = require('fs')
const path = require('path')
const { UseCaseRequestTemplate, UseCaseRequestHandlerTemplate, UseCaseRequestValidatorTemplate } = require('../templates')
const { capitalizeFirstLetter } = require('../../shared-kernel/utils')

const BASE_PATH = '../../../src/backend/Ineditta.Application'

class UseCaseAction {
  constructor() {
    this.basePath = BASE_PATH
  }
  
  criar(moduleName, actionName, subMoluleName = null) {
    const upperModuleName = capitalizeFirstLetter(moduleName)
    const upperActionName = capitalizeFirstLetter(actionName)
    let upperSubModuleName = null

    const useCaseRequestTemplate = UseCaseRequestTemplate.create(moduleName, actionName, subMoluleName)
    const useCaseRequestValidatorTemplate = UseCaseRequestValidatorTemplate.create(moduleName, actionName, subMoluleName)
    const useCaseRequestHandlerTemplate = UseCaseRequestHandlerTemplate.create(moduleName, actionName, subMoluleName)

    const modulePath = this._getWithBasePath(upperModuleName)
    this._verifyAndCreateFolder(modulePath)

    
    let moduleActionsPath = `${upperModuleName}`
    if (subMoluleName) {
      upperSubModuleName = capitalizeFirstLetter(subMoluleName)
      moduleActionsPath = `${upperModuleName}/${upperSubModuleName}`
    }

    if (subMoluleName) {      
      const subModulePath = this._getWithBasePath(moduleActionsPath)
      this._verifyAndCreateFolder(subModulePath)
      
      const subModuleUseCasePath = this._getWithBasePath(`${moduleActionsPath}/UseCases`)
      this._verifyAndCreateFolder(subModuleUseCasePath)
    } else {      
      const moduleUseCasePath = this._getWithBasePath(`${moduleActionsPath}/UseCases`)
      this._verifyAndCreateFolder(moduleUseCasePath)
    }

    const baseFileName = `${upperModuleName}${upperActionName}`
    const useCasePath = `${moduleActionsPath}/UseCases`

    const useCaseRequestPath = this._getWithBasePath(`${useCasePath}/${baseFileName}Request.cs`)
    if (fs.existsSync(useCaseRequestPath)) return
    this._createFile(useCaseRequestPath, useCaseRequestTemplate)

    const useCaseRequestValidatorPath = this._getWithBasePath(`${useCasePath}/${baseFileName}RequestValidator.cs`)
    if (fs.existsSync(useCaseRequestValidatorPath)) return
    this._createFile(useCaseRequestValidatorPath, useCaseRequestValidatorTemplate)

    const useCaseRequestHandlerPath = this._getWithBasePath(`${useCasePath}/${baseFileName}RequestHandler.cs`)
    if (fs.existsSync(useCaseRequestHandlerPath)) return
    this._createFile(useCaseRequestHandlerPath, useCaseRequestHandlerTemplate)
  }

  _getWithBasePath(currentPath) {
    return this._getPath(`${this.basePath}/${currentPath}`)
  }

  _getPath(currentPath) {
    return path.join(__dirname, currentPath)
  }
  
  _verifyAndCreateFolder(folder) {
    if (!fs.existsSync(folder)) {
      fs.mkdirSync(folder)
    }
  }

  _createFile(filePath, serviceTemplate) {
    fs.writeFile(filePath, serviceTemplate, (err) => {
      if (err) {
        return console.error('Erro ao escrever no arquivo:', err)
      }
    })
  }
}

module.exports = {
  UseCaseAction
}