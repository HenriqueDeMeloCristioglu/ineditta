const fs = require('fs')
const path = require('path')

const BASE_PATH = '../../../sistema/src/js/modules'

class BaseCrudAction {
  basePath = BASE_PATH

  criar(moduleName, template, actionFileName) {
    const modulesPath = this._getPath(this.basePath)
    
    const folder = path.join(modulesPath, moduleName)
    this._verifyAndCreateFolder(folder)

    const featuresPath = this._getWithBasePath(`${moduleName}/features`)
    this._verifyAndCreateFolder(featuresPath)

    const actionsPath = this._getWithBasePath(`${moduleName}/features/actions`)
    this._verifyAndCreateFolder(actionsPath)

    const filePath = this._getWithBasePath(`${moduleName}/features/actions/${actionFileName}-${moduleName}.js`)
    if (fs.existsSync(filePath)) return

    fs.writeFile(filePath, template, (err) => {
      if (err) {
        return console.error('Erro ao escrever no arquivo:', err)
      }
    })
  }

  _getWithBasePath(currentPath) {
    return this._getPath(`${this.basePath}/${currentPath}`)
  }
  
  _verifyAndCreateFolder(folder) {
    if (!fs.existsSync(folder)) {
      fs.mkdirSync(folder)
    }
  }

  _getPath(currentPath) {
    return path.join(__dirname, currentPath)
  }
}

module.exports = {
  BaseCrudAction
}