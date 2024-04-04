const { capitalizeFirstLetter } = require("../../../shared-kernel/utils")

class UseCaseRequestValidatorTemplate {
  static create(moduleName, actionName, subModule = null) {
    const upperModuleName = capitalizeFirstLetter(moduleName)
    const upperActionName = capitalizeFirstLetter(actionName)

    let template = `using FluentValidation;`
    
    if (subModule) {
      const upperSubModuleName = capitalizeFirstLetter(subModule)

      template += `

namespace Ineditta.Application.${upperModuleName}.${upperSubModuleName}.UseCases.${upperActionName}
      `
    } else {
      template += `

using Ineditta.Application.${upperModuleName}.UseCases.${upperActionName};

namespace Ineditta.Application.${upperModuleName}.UseCases.${upperActionName}`
    }

    template += `
{
    public class ${upperActionName}${upperModuleName}RequestValidator : AbstractValidator<${upperActionName}${upperModuleName}Request>
    {
        public ${upperActionName}${upperModuleName}RequestValidator()
        {
        }
    }
}`

    return template
  }
}

module.exports = {
  UseCaseRequestValidatorTemplate
}
