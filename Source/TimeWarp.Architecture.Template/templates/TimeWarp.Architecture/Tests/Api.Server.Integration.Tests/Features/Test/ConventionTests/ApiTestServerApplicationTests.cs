﻿namespace ApiTestServerApplication_;

[TestTag("ApiTestServerApplication")]
public class Should
{
  public Should
  (
    ApiTestServerApplication aApiTestServerApplication
  )
  {
    Guard.Argument(aApiTestServerApplication).NotNull();
  }

  public void Start_Without_Exception() => true.Should().BeTrue();
  
  [Skip("This test runs forever to allow me to manually test if servers are running properly.  Normally needs to be skipped as it will never completed")]
  public async Task RunForever()
  {
    await Task.Delay(int.MaxValue);
    Console.WriteLine("done");
  }
}
