namespace TimeWarp.Architecture.Testing;

/// <summary>
/// Replaces the default ClientLoaderConfiguration
/// To reduce the delay from 10 seconds to 10 milliseconds.
/// So tests run much faster.
/// </summary>
[NotTest]
public class ClientLoaderTestConfiguration : IClientLoaderConfiguration
{
  /// <summary>
  /// Shortent the Delay for tests
  /// </summary>
  public TimeSpan DelayTimeSpan => TimeSpan.FromMilliseconds(10);
}
