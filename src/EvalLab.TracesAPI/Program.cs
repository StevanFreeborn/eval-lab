using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestDecompression();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRequestDecompression();

app.MapOpenApi();

app.MapPost("/v1/traces", ([FromBody] JsonElement json) =>
{
  var value = json.GetRawText();
  return Results.Ok();
});

// app.UseHttpsRedirection();

app.Run();