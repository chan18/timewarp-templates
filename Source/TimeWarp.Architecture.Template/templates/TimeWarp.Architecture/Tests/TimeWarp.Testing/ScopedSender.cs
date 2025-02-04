﻿namespace TimeWarp.Architecture.Testing;

/// <summary>
/// This is an implementation of MediatR's ISender Interface
/// that wraps calls to Send in a <see cref="IServiceScope"/>.
/// </summary>
[NotTest]
public class ScopedSender : ISender
{
  private readonly IServiceScopeFactory ServiceScopeFactory;

  public ScopedSender(IServiceProvider aServiceProvider)
  {
    ServiceScopeFactory = aServiceProvider.GetService<IServiceScopeFactory>();
  }

  public IAsyncEnumerable<TResponse> CreateStream<TResponse>
  (
    IStreamRequest<TResponse> aStreamRequest,
    CancellationToken aCancellationToken = default
  ) => throw new NotImplementedException();

  public IAsyncEnumerable<object> CreateStream
  (
    object aRequest,
    CancellationToken aCancellationToken = default
  ) => throw new NotImplementedException();

  public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
  {
    return ExecuteInScope
    (
      aServiceProvider =>
      {
        IMediator mediator = aServiceProvider.GetService<IMediator>();

        return mediator.Send(request);
      }
    );
  }

  public Task<object> Send(object aRequest, CancellationToken aCancellationToken = default)
  {
    return ExecuteInScope
    (
      aServiceProvider =>
      {
        IMediator mediator = aServiceProvider.GetService<IMediator>();

        return mediator.Send(aRequest);
      }
    );
  }

  public Task<TResponse> Send<TResponse>(IRequest<TResponse> aRequest, CancellationToken aCancellationToken = default)
  {
    return ExecuteInScope
    (
      aServiceProvider =>
      {
        IMediator mediator = aServiceProvider.GetService<IMediator>();

        return mediator.Send(aRequest);
      }
    );
  }

  internal async Task<T> ExecuteInScope<T>(Func<IServiceProvider, Task<T>> aAction)
  {
    using IServiceScope serviceScope = ServiceScopeFactory.CreateScope();
    return await aAction(serviceScope.ServiceProvider).ConfigureAwait(false);
  }

  internal async Task ExecuteInScope(Func<IServiceProvider, Task> aAction)
  {
    using IServiceScope serviceScope = ServiceScopeFactory.CreateScope();
    await aAction(serviceScope.ServiceProvider).ConfigureAwait(false);
  }
}
