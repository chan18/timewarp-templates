namespace TimeWarp.Architecture.Pages;

using static TimeWarp.Architecture.Features.Applications.Spa.ApplicationState;

[Page("/Counter")]
public partial class CounterPage : BaseComponent
{
  private async Task ButtonClick() =>
    await Send(new RouteState.ChangeRouteAction { NewRoute = "/" }).ConfigureAwait(false);

  private async Task ResetButtonClick() => await Send(new ResetStoreAction()).ConfigureAwait(false);
}
