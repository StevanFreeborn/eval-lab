using Aspire.Hosting.Lifecycle;

using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

var dockerHostName = "host.docker.internal";
var aspireOtlpEndpoint = builder.Configuration["DOTNET_DASHBOARD_OTLP_ENDPOINT_URL"]?
  .Replace("localhost", dockerHostName, StringComparison.OrdinalIgnoreCase);
var aspireOtlpApiKey = builder.Configuration["AppHost:OtlpApiKey"];

var collector = builder.AddContainer("otel-collector", "ghcr.io/open-telemetry/opentelemetry-collector-releases/opentelemetry-collector-contrib")
  .WithBindMount("../EvalLab.Containers/OtelCollector/config.yml", "/etc/otelcol-contrib/config.yaml")
  .WithEndpoint(port: 4317, targetPort: 4317, name: "grpc", scheme: "http")
  .WithEndpoint(port: 4318, targetPort: 4318, name: "http", scheme: "http")
  .WithEnvironment("ASPIRE_OTLP_ENDPOINT", aspireOtlpEndpoint)
  .WithEnvironment("ASPIRE_OTLP_API_KEY", aspireOtlpApiKey);

collector.ApplicationBuilder.Services.TryAddLifecycleHook<ReplaceOtelEndpointHost>();

var mongo = builder.AddMongoDB("mongo")
  .WithDataVolume(name: "mongo-data")
  .WithMongoExpress(containerName: "mongo-express")
  .WaitFor(collector);

var mongodb = mongo.AddDatabase("mongodb", databaseName: "evallab");

var api = builder.AddProject<Projects.EvalLab_API>("api")
  .WithReference(mongodb)
  .WaitFor(mongodb)
  .WithExternalHttpEndpoints();

collector.WithReference(api);

builder.AddNpmApp("client", "../EvalLab.Client")
  .WithReference(api)
  .WaitFor(api)
  .WithHttpsEndpoint(env: "PORT", port: 4000)
  .WithExternalHttpEndpoints();

builder.Build().Run();

class ReplaceOtelEndpointHost(ILogger<ReplaceOtelEndpointHost> logger) : IDistributedApplicationLifecycleHook
{
  private readonly ILogger<ReplaceOtelEndpointHost> _logger = logger;

  public Task AfterEndpointsAllocatedAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken)
  {
    var resources = appModel.GetProjectResources();
    var collectorResource = appModel.Resources.OfType<ContainerResource>().FirstOrDefault(r => r.Name == "otel-collector");

    if (collectorResource is null)
    {
      _logger.LogWarning("No collector resource found");
      return Task.CompletedTask;
    }

    var endpoint = collectorResource.GetEndpoint("grpc");

    if (endpoint is null)
    {
      _logger.LogWarning("No endpoint for the collector");
      return Task.CompletedTask;
    }

    if (resources.Any() is false)
    {
      _logger.LogInformation("No resources to add Environment Variables to");
    }

    foreach (var resourceItem in resources)
    {

      if (resourceItem is null)
      {
        _logger.LogWarning("No resource found");
        continue;
      }

      _logger.LogDebug("Forwarding Telemetry for {ResourceName} to the collector", resourceItem.Name);

      resourceItem.Annotations.Add(new EnvironmentCallbackAnnotation(context =>
      {
        var envKey = "OTEL_EXPORTER_OTLP_ENDPOINT";

        context.EnvironmentVariables.Remove(envKey);
        context.EnvironmentVariables.Add(envKey, endpoint.Url);
      }));
    }

    return Task.CompletedTask;
  }
}