const fs = require('fs')
const path = require('path')
const { ServiceTemplate } = require('../templates')

const BASE_PATH = '../../../sistema/src/js/services'

class ServiceAction {
  constructor() {
    this.basePath = BASE_PATH
  }
  
  criar(moduleName) {
    const serviceTemplate = ServiceTemplate.create(moduleName)
    
    const filePath = this._getWithBasePath(`${moduleName}-service.js`)

    if (fs.existsSync(filePath)) return

    fs.writeFile(filePath, serviceTemplate, (err) => {
      if (err) {
        return console.error('Erro ao escrever no arquivo:', err)
      }
    })
  }

  _getWithBasePath(currentPath) {
    return this._getPath(`${this.basePath}/${currentPath}`)
  }

  _getPath(currentPath) {
    return path.join(__dirname, currentPath)
  }
}

module.exports = {
  ServiceAction
}