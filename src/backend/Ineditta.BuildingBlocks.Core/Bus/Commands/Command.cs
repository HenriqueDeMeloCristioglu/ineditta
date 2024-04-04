using CSharpFunctionalExtensions;

using MediatR;

namespace Ineditta.BuildingBlocks.Core.Bus.Commands
{
    public class Command : Message, IRequest<Result>
    {
    }
}