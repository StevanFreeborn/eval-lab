var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo").WithMongoExpress(containerName: "mongo-express");
var mongodb = mongo.AddDatabase("mongodb", databaseName: "eval-lab");

var api = builder.AddProject<Projects.EvalLab_API>("api")
  .WithReference(mongodb)
  .WaitFor(mongodb)
  .WithExternalHttpEndpoints();

builder.AddNpmApp("client", "../EvalLab.Client")
  .WithReference(api)
  .WaitFor(api)
  .WithHttpsEndpoint(env: "PORT", port: 4000)
  .WithExternalHttpEndpoints()
  .PublishAsDockerFile();

builder.Build().Run();
