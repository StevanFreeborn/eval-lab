using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.Json;

using EvalLab.ServiceDefaults;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

using OpenTelemetry.Proto.Trace.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestDecompression();

builder.Services.AddProblemDetails();

builder.AddServiceDefaults();

ClassMap.Register();

builder.AddMongoDBClient("mongodb");

builder.Services.AddOpenApi();

builder.Services.AddCors();

var app = builder.Build();

app.UseRequestDecompression();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseStatusCodePages();

// Collector is sending traces to the OTLP endpoint
// over http...problem with container talking to
// host.docker.internal...need to come back to this
// app.UseHttpsRedirection();

app.MapOpenApi();

app.MapDefaultEndpoints();

app.MapPost("/evaluations", async (IMongoClient client, [FromBody] AddEvaluationDto dto) =>
{
  if (dto.TryValidate(out var results) is false)
  {
    return Results.ValidationProblem(results.ToErrors());
  }

  var evaluation = dto.ToEvaluation();
  await client.GetDatabase("evallab").GetCollection<Evaluation>("evaluations").InsertOneAsync(evaluation);
  return Results.Created($"/evaluations/{evaluation.Id}", evaluation.ToDto());
});

app.MapGet("/evaluations", async (IMongoClient client, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50) =>
{
  Activity.Current?.SetTag("evallab.evaluation.run", true);

  var totalFacet = AggregateFacet.Create(
    "count",
    PipelineDefinition<Evaluation, AggregateCountResult>.Create(
      [PipelineStageDefinitionBuilder.Count<Evaluation>()]
    )
  );


  var itemsFacet = AggregateFacet.Create(
    "items",
    PipelineDefinition<Evaluation, Evaluation>.Create(
      [
        PipelineStageDefinitionBuilder.Skip<Evaluation>((pageNumber - 1) * pageSize),
        PipelineStageDefinitionBuilder.Limit<Evaluation>(pageSize)
      ]
    )
  );

  var aggregation = await client.GetDatabase("evallab").GetCollection<Evaluation>("evaluations").Aggregate()
    .SortByDescending(e => e.CreatedDate)
    .Facet(totalFacet, itemsFacet)
    .FirstOrDefaultAsync();

  var countOutput = aggregation.Facets.First(f => f.Name == "count").Output<AggregateCountResult>();
  var totalItems = countOutput.Count is 0 ? 0 : countOutput[0].Count;
  var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
  var items = aggregation.Facets.First(f => f.Name == "items").Output<Evaluation>();
  var itemsDto = items.Select(e => e.ToDto());

  return Results.Ok(new PageDto<EvaluationDto>(pageNumber, pageSize, totalPages, totalItems, [.. itemsDto]));
});

app.MapGet("/evaluations/{id}", async (IMongoClient client, string id) =>
{
  var evaluation = await client.GetDatabase("evallab").GetCollection<Evaluation>("evaluations").Find(e => e.Id == id).FirstOrDefaultAsync();
  return evaluation is not null ? Results.Ok(evaluation.ToDto()) : Results.NotFound();
});

app.MapDelete("/evaluations/{id}", async (IMongoClient client, string id) =>
{
  var result = await client.GetDatabase("evallab").GetCollection<Evaluation>("evaluations").DeleteOneAsync(e => e.Id == id);
  return result.DeletedCount is 1 ? Results.NoContent() : Results.NotFound();
});

app.MapPost("/v1/traces", ([FromBody] JsonElement traceData) =>
{
  var jsonText = traceData.GetRawText();
  var trace = TracesData.Parser.ParseJson(jsonText);
  return Results.Ok();
});

app.Run();

record EvaluationDto(
  string Id,
  string Name,
  string Description,
  DateTime CreatedDate,
  DateTime UpdatedDate
)
{
  public EvaluationDto(Evaluation evaluation) : this(
    evaluation.Id.ToString(),
    evaluation.Name,
    evaluation.Description,
    evaluation.CreatedDate,
    evaluation.UpdatedDate
  )
  { }
}

record AddEvaluationDto(string Name, string Description)
{
  public bool TryValidate(out List<ValidationResult> results)
  {
    results = [];

    if (string.IsNullOrWhiteSpace(Name))
    {
      results.Add(new ValidationResult("Name is required", [nameof(Name)]));
    }

    return results.Count is 0;
  }

  public Evaluation ToEvaluation() => new() { Name = Name, Description = Description };
}

record PageDto<T>(int PageNumber, int PageSize, int TotalPages, long TotalItems, T[] Items);

class Evaluation
{
  public string Id { get; init; } = null!;
  public string Name { get; init; } = string.Empty;
  public string Description { get; init; } = string.Empty;
  public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
  public DateTime UpdatedDate { get; init; } = DateTime.UtcNow;
}

static class EvaluationExtensions
{
  public static EvaluationDto ToDto(this Evaluation evaluation) => new(evaluation);
}

static class ValidationResultsExtensions
{
  public static Dictionary<string, string[]> ToErrors(this List<ValidationResult> results) => results.ToDictionary(r => r.MemberNames.First(), r => new[] { r.ErrorMessage! });
}

static class ClassMap
{
  public static void Register()
  {
    BsonClassMap.RegisterClassMap<Evaluation>(cm =>
    {
      cm.AutoMap();
      cm.SetIgnoreExtraElements(true);
      cm.MapIdProperty(e => e.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
      cm.MapProperty(e => e.Name).SetElementName("name");
      cm.MapProperty(e => e.Description).SetElementName("description");
      cm.MapProperty(e => e.CreatedDate).SetElementName("createdDate");
      cm.MapProperty(e => e.UpdatedDate).SetElementName("updatedDate");
    });
  }
}