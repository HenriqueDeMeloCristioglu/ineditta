const { capitalizeFirstLetter } = require("../../../shared-kernel/utils")

class BaseCrudTemplate {
  static create(moduleName, actionName) {
    const upperModuleName = capitalizeFirstLetter(moduleName)

    return `import { ApiService } from "../../../../core/index"
import Result from "../../../../core/result"
import { ${upperModuleName}Service } from "../../../../services"

const ${moduleName}Service = new ${upperModuleName}Service(new ApiService())

export async function ${actionName}${upperModuleName}(id) {
  const result = await ${moduleName}Service.${actionName}(id)

  if (result.isFailure()) {
    return result
  }

  return Result.success()
}`
  }
}


module.exports = {
  BaseCrudTemplate
}