namespace TimeWarp.Architecture.Features.Applications;
internal partial class ApplicationState
{
  [TrackProcessing]
  public record FiveSecondTaskAction : BaseAction { }
}
