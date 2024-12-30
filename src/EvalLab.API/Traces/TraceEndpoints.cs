using System.Text.Json;

using EvalLab.API.Data;
using EvalLab.API.Pipelines;

using Microsoft.AspNetCore.Mvc;

using OpenTelemetry;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Trace.V1;

namespace EvalLab.API.Traces;

static class TraceEndpoints
{
  public static WebApplication MapTraceEndpoints(this WebApplication app)
  {
    app.MapPost("/v1/traces", async ([FromBody] JsonElement data, [FromServices] IRepository<Trace> repo) =>
    {
      using var scope = SuppressInstrumentationScope.Begin();

      var jsonText = data.GetRawText();
      var traceData = TracesData.Parser.ParseJson(jsonText);

      var runIds = traceData.ResourceSpans
        .SelectMany(rs => rs.ScopeSpans)
        .SelectMany(ss => ss.Spans)
        .SelectMany(s => s.Attributes)
        .Where(a => a.Key == PipelineRun.RunAttribute)
        .Select(a => a.Value.StringValue)
        .Select(runId => runId.Split(PipelineRun.RunIdPrefix)[1])
        .Distinct()
        .ToArray();

      if (runIds.Length is 0)
      {
        return Results.BadRequest("No runIds found");
      }

      if (runIds.Length is not 1)
      {
        return Results.BadRequest("Multiple runIds found");
      }

      var trace = traceData.ToTrace(runIds[0]);

      var existingTrace = await repo.GetAsync(FilterSpecification<Trace>.From(t => t.PipelineRunId == trace.PipelineRunId));

      if (existingTrace is not null)
      {
        return Results.Conflict("Trace already created for this run");
      }

      await repo.CreateAsync(trace);

      return Results.Ok();
    });

    var group = app.MapGroup("/traces");

    group.MapGet("{runId}", async (string runId, [FromServices] IRepository<Trace> repo) =>
    {
      var trace = await repo.GetAsync(FilterSpecification<Trace>.From(t => t.PipelineRunId == runId));
      return trace is not null ? Results.Ok(TraceDto.From(trace)) : Results.NotFound();
    });

    return app;
  }
}