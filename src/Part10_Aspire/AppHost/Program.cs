// Aspire AppHost — C# resource graph (dev-time orchestration)
// Production still runs on K8s/ACA; AppHost is not the production runtime.

var builder = DistributedApplication.CreateBuilder(args);

// Option A: declare containers in AppHost (Aspire starts them)
// Option B (this lab default): also document connecting to already-running /Project/docker stack
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("campusdb");

var redis = builder.AddRedis("redis");
var rabbit = builder.AddRabbitMQ("rabbitmq").WithManagementPlugin();

var api = builder.AddProject<Projects.Api>("campus-api")
    .WithReference(postgres)
    .WithReference(redis)
    .WithReference(rabbit)
    .WaitFor(postgres)
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317")
    .WithExternalHttpEndpoints();

builder.Build().Run();
