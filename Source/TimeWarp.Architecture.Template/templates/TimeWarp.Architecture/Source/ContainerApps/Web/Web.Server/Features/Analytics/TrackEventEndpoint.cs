﻿namespace TimeWarp.Architecture.Features.Analytics;

using static TimeWarp.Architecture.Features.Analytics.Contracts.TrackEvent;
public class TrackEventEndpoint : BaseEndpoint<Command, Response>
{
  /// <summary>
  /// Track events in analytics
  /// </summary>
  /// <param name="command"></param>
  /// <returns></returns>
  [HttpPost(Command.Route)]
  [SwaggerOperation(Tags = new[] { FeatureAnnotations.FeatureGroup })]
  [ProducesResponseType(typeof(Response), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
  public Task<IActionResult> Process([FromBody] Command command) => Send(command);
}
