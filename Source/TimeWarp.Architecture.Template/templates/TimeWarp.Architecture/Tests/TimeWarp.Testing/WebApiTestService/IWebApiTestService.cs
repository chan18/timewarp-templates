﻿namespace TimeWarp.Architecture.Testing;

public interface IWebApiTestService
{
  /// <summary>
  /// Confirm that the endpoint for the request will return a BadRequest Status and
  /// explicitly contain the <paramref name="aAttributeName"/> in the error message
  /// </summary>
  /// <typeparam name="TResponse"></typeparam>
  /// <param name="aApiRequest"></param>
  /// <param name="aAttributeName"></param>
  /// <returns></returns>
  Task ConfirmEndpointValidationError<TResponse>
  (
    IApiRequest aApiRequest,
    string aAttributeName
  );

  /// <summary>
  /// Return the Response object by getting it as json and deseralizing it/>
  /// </summary>
  /// <typeparam name="TResponse"></typeparam>
  /// <param name="aRequest"></param>
  /// <returns></returns>
  Task<TResponse> GetResponse<TResponse>(IApiRequest aApiRequest);
}
