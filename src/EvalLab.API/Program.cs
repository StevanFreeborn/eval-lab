using EvalLab.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddMongoDBClient("mongodb");

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.Run();