const { capitalizeFirstLetter } = require("../../../shared-kernel/utils")

class UseCaseRequestTemplate {
  static create(moduleName, actionName, subModule = null) {
    const upperModuleName = capitalizeFirstLetter(moduleName)
    const upperActionName = capitalizeFirstLetter(actionName)

    let template = `using CSharpFunctionalExtensions;

using MediatR;`
    
    if (subModule) {
      const upperSubModuleName = capitalizeFirstLetter(subModule)

      template += `

namespace Ineditta.Application.${upperModuleName}.${upperSubModuleName}.UseCases.${upperActionName}`
    } else {
      template += `

namespace Ineditta.Application.${upperModuleName}.UseCases.${upperActionName}`
    }

    template += `
{
    public class ${upperActionName}${upperModuleName}Request : IRequest<Result>
    {
    }
}`

    return template
  }
}

module.exports = {
  UseCaseRequestTemplate
}