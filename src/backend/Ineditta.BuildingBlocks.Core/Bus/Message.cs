using MediatR;

namespace Ineditta.BuildingBlocks.Core.Bus
{
    public abstract class Message: IBaseRequest
    {
        protected Message()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
        public Guid Id { get; protected set; }
        public DateTime CreationDate { get; protected set; }
    }
}
