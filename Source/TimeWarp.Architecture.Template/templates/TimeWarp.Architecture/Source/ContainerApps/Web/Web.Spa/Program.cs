namespace TimeWarp.Architecture.Web.Spa;

using BlazorState;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using MediatR;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeterLeslieMorris.Blazor.Validation;
using ProtoBuf.Grpc.Client;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using TimeWarp.Architecture.Analyzer;
using TimeWarp.Architecture.Components;
using TimeWarp.Architecture.Configuration;
using TimeWarp.Architecture.Features.Applications;
using TimeWarp.Architecture.Features.ClientLoaders;
using TimeWarp.Architecture.Features.EventStreams;
using TimeWarp.Architecture.Features.Superheros;
using ServiceCollection = Configuration.ServiceCollection;

public class Program
{
  public static void ConfigureServices(IServiceCollection aServiceCollection, IConfiguration aConfiguration)
  {
    ConfigureSettings(aServiceCollection, aConfiguration);
    aServiceCollection.AddBlazorState
    (
      (aOptions) =>
      {
#if ReduxDevToolsEnabled
        aOptions.UseReduxDevToolsBehavior = true;
#endif
        aOptions.Assemblies =
          new Assembly[]
          {
              typeof(Program).GetTypeInfo().Assembly,
          };
      }
    );

    aServiceCollection.AddFormValidation
    (
      aValidationConfiguration => aValidationConfiguration.AddFluentValidation(typeof(Program).Assembly)
    );

    aServiceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(ProcessingBehavior<,>));
    aServiceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(EventStreamBehavior<,>));
    aServiceCollection.AddScoped<ClientLoader>();
    aServiceCollection.AddScoped<IClientLoaderConfiguration, ClientLoaderConfiguration>();
    aServiceCollection.AddScoped<WebApiService>();
    // Set the JSON serializer options
    aServiceCollection.Configure<JsonSerializerOptions>
    (
      options =>
      {
        //options.PropertyNameCaseInsensitive = false;
        options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        //options.WriteIndented = true;
      }
    );

    ConfigureGrpc(aServiceCollection);

#if DEBUG
    new ProjectAnlayzer().Analyze();
#endif
  }

  private static void ConfigureSettings(IServiceCollection aServiceCollection, IConfiguration aConfiguration)
  {
    aServiceCollection
      .ConfigureOptions<ServiceCollection, ServiceCollectionValidator>(aConfiguration);

    //aServiceCollection.ValidateOptions();
  }

  private static void ConfigureGrpc(IServiceCollection aServiceCollection)
  {
    aServiceCollection.AddSingleton
    (
      aServiceProvider =>
      {
        IConfiguration configuration = aServiceProvider.GetRequiredService<IConfiguration>();
        const string ServiceName = "grpc-server";
        string grpcUrl = GetServiceUri(configuration, ServiceName);

        if (string.IsNullOrEmpty(grpcUrl))
        {
          throw new Exception("No grpc-server address found in configuration");
        }

        Console.WriteLine($"grpcUrl:{grpcUrl}");

        // Create a channel with a GrpcWebHandler that is addressed to the backend server.
        //
        // GrpcWebText is used because server streaming requires it. If server streaming is not used in your app
        // then GrpcWeb is recommended because it produces smaller messages.
        var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler());

        return GrpcChannel.ForAddress
        (
          grpcUrl,
          new GrpcChannelOptions
          {
            HttpHandler = httpHandler,
            //CompressionProviders = ...,
            //Credentials = ...,
            //DisposeHttpClient = ...,
            //HttpClient = ...,
            //LoggerFactory = ...,
            //MaxReceiveMessageSize = ...,
            //MaxSendMessageSize = ...,
            //ThrowOperationCanceledOnCancellation = ...,
          }
        );
      }
    );

    aServiceCollection.AddSingleton<ISuperheroService>
    (
      aServiceProvider =>
      {
        GrpcChannel grpcChannel = aServiceProvider.GetRequiredService<GrpcChannel>();
        return grpcChannel.CreateGrpcService<ISuperheroService>();
      }
    );

  }

  private static string GetServiceUri(IConfiguration aConfiguration, string aServiceName)
  {
    var uriBuilder = new UriBuilder
    {
      Scheme = aConfiguration.GetValue<string>($"service:{aServiceName}:protocol"),
      Host = aConfiguration.GetValue<string>($"service:{aServiceName}:host"),
      Port = aConfiguration.GetValue<int>($"service:{aServiceName}:port")
    };

    string serviceUri = aConfiguration.GetServiceUri(aServiceName)?.AbsoluteUri ?? uriBuilder.ToString();

    return serviceUri;
  }

  public static Task Main(string[] aArgumentArray)
  {
    var builder = WebAssemblyHostBuilder.CreateDefault(aArgumentArray);
    builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
    builder.RootComponents.Add<App>("#app");
    builder.Services.AddScoped
      (_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

    ConfigureServices(builder.Services, builder.Configuration);

    WebAssemblyHost host = builder.Build();
    return host.RunAsync();
  }
}
