using AnthropicClient;

using EvalLab.API.Data;
using EvalLab.API.Demo;
using EvalLab.API.Evaluations;
using EvalLab.API.Json;
using EvalLab.API.Pipelines;
using EvalLab.API.Traces;
using EvalLab.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new SuccessCriteriaConverter()));

builder.Services.AddRequestDecompression();

builder.Services.AddProblemDetails();

builder.AddServiceDefaults();

builder.AddMongoDBClient("mongodb");

await builder.Services.AddSingleton<MongoDbContext>().AddIndexes();
builder.Services.AddScoped<IRepository<Pipeline>, MongoPipelineRepository>();
builder.Services.AddScoped<IRepository<PipelineRun>, MongoPipelineRunRepository>();
builder.Services.AddScoped<IRepository<Evaluation>, MongoEvaluationRepository>();
builder.Services.AddScoped<IRepository<EvaluationRun>, MongoEvaluationRunRepository>();
builder.Services.AddScoped<IRepository<Trace>, MongoTraceRepository>();

builder.Services.AddSingleton<IEvaluationRunQueue, EvaluationRunQueue>();
builder.Services.AddSingleton<IEvaluationRunProcessor, EvaluationRunProcessor>();
builder.Services.AddHostedService<EvaluationRunQueueService>();

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
app.MapEvaluationRunEndpoints();

app.MapTraceEndpoints();

app.MapPipelineEndpoints();
app.MapPipelineRunEndpoints();

app.MapDemoEndpoints();

app.Run();