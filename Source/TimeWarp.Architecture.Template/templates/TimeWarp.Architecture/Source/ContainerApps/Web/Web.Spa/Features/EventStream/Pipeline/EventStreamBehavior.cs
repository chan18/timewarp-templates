namespace TimeWarp.Architecture.Features.EventStreams.Spa;

using static TimeWarp.Architecture.Features.EventStreams.Spa.EventStreamState;

/// <summary>
/// Every event that comes through the pipeline adds an object to the EventStreamState
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <remarks>To avoid infinite recursion don't add AddEvent to the event stream</remarks>
public class EventStreamBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull, IAction
{
  private readonly ILogger Logger;
  private readonly ISender Sender;
  public Guid Guid { get; } = Guid.NewGuid();

  public EventStreamBehavior
  (
    ILogger<EventStreamBehavior<TRequest, TResponse>> logger,
    ISender sender
  )
  {
    Logger = logger;
    Sender = sender;
    Logger.LogDebug($"{GetType().Name}: Constructor");
  }

  public async Task<TResponse> Handle
  (
    TRequest aRequest,
    RequestHandlerDelegate<TResponse> aNext,
    CancellationToken aCancellationToken
  )
  {
    Guard.Argument(aNext, nameof(aNext)).NotNull();

    await AddEventToStream(aRequest, "Start").ConfigureAwait(false);
    TResponse newState = await aNext().ConfigureAwait(false);
    await AddEventToStream(aRequest, "Completed").ConfigureAwait(false);
    return newState;
  }

  private async Task AddEventToStream(TRequest aRequest, string aTag)
  {
    if (aRequest is not AddEventAction) //Skip to avoid recursion
    {
      var addEventAction = new AddEventAction();
      string requestTypeName = aRequest.GetType().Name;

      if (aRequest is BaseRequest request)
      {
        addEventAction.Message = $"{aTag}:{requestTypeName}";
      }
      else
      {
        addEventAction.Message = $"{aTag}:{requestTypeName}";
      }
      await Sender.Send(addEventAction).ConfigureAwait(false);
    }
  }
}
