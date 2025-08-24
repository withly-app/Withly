using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pgUsername = builder.AddParameter("pgUsername", secret: true);
var pgPassword = builder.AddParameter("pgPassword", secret: true);

var postgres = builder.AddPostgres("postgres", pgUsername, pgPassword)
    .WithImage("postgres")
    .WithDataVolume("withly-postgres-data")
    .WithEndpoint(port: 15432, targetPort: 5432, name: "withly-postgres-endpoint");

var postgresDb = postgres.AddDatabase("WithlyDB", "WithlyDB");

builder.AddProject<Withly_API>("API")
    .WithReference(postgresDb, "DefaultConnection");

builder.Build().Run();