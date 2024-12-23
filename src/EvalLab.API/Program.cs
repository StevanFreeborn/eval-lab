using System.Text.Json;

using AnthropicClient;

using EvalLab.API.Data;
using EvalLab.API.Demo;
using EvalLab.API.Evaluations;
using EvalLab.API.Pipelines;
using EvalLab.ServiceDefaults;

using Microsoft.AspNetCore.Mvc;

using OpenTelemetry.Proto.Trace.V1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestDecompression();

builder.Services.AddProblemDetails();

builder.AddServiceDefaults();

builder.AddMongoDBClient("mongodb");

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IRepository<Pipeline>, MongoPipelineRepository>();
builder.Services.AddScoped<IRepository<Evaluation>, MongoEvaluationRepository>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAnthropicApiClient>(sp =>
{
  var config = sp.GetRequiredService<IConfiguration>();
  var httpClient = sp.GetRequiredService<HttpClient>();
  var key = config.GetSection("Anthropic").GetValue<string>("ApiKey");

  if (string.IsNullOrWhiteSpace(key))
  {
    throw new InvalidOperationException("Anthropic API key is required");
  }

  return new AnthropicApiClient(key, httpClient);
});

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

app.MapEvaluationEndpoints();

app.MapPost("/v1/traces", ([FromBody] JsonElement traceData) =>
{
  var jsonText = traceData.GetRawText();
  var trace = TracesData.Parser.ParseJson(jsonText);
  return Results.Ok();
});

app.MapPipelineEndpoints();

app.MapDemoEndpoints();

app.Run();




