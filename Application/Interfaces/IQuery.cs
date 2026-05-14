using MediatR;

namespace Application.Interfaces
{
    // Base interface for all queries
    // Using MediatR's built-in capabilities, but defining a structure here
    // to explicitly model the CQRS pattern interfaces for clarity.
    public interface IQuery<TResult> : IRequest<TResult> { }
}