const { capitalizeFirstLetter } = require("../../../shared-kernel/utils")

class UseCaseRequestHandlerTemplate {
  static create(moduleName, actionName, subModule = null) {
    const upperModuleName = capitalizeFirstLetter(moduleName)
    const upperActionName = capitalizeFirstLetter(actionName)

    let template = `using CSharpFunctionalExtensions;

using Ineditta.BuildingBlocks.Core.Database;
using Ineditta.BuildingBlocks.Core.Domain.Handlers;

using MediatR;`
    
    if (subModule) {
      const upperSubModuleName = capitalizeFirstLetter(subModule)

      template += `

namespace Ineditta.Application.${upperModuleName}.${upperSubModuleName}.UseCases.${upperActionName}      
      `
    } else {
      template += `

namespace Ineditta.Application.${upperModuleName}.UseCases.${upperActionName}`
    }

    template += `
{
    public class ${upperActionName}${upperModuleName}RequestHandler : BaseCommandHandler, IRequestHandler<${upperActionName}${upperModuleName}Request, Result>
    {
        public ${upperActionName}${upperModuleName}RequestHandler(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<Result> Handle(${upperActionName}${upperModuleName}Request request, CancellationToken cancellationToken)
        {
          await Task.CompletedTask;
          
          return Result.Success();
        }
    }
}`

    return template
  }
}

module.exports = {
  UseCaseRequestHandlerTemplate
}
