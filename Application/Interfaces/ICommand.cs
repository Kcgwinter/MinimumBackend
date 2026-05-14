using MediatR;

namespace Application.Interfaces;

/// <summary>
/// Base interface for all commands that return a result.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse> { }
