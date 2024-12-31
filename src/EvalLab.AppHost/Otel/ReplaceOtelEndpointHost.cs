using Aspire.Hosting.Lifecycle;

using Microsoft.Extensions.Logging;

namespace EvalLab.AppHost.Otel;

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