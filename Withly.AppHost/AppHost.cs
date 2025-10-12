using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pgUsername = builder.AddParameter("pgUsername", secret: true);
var pgPassword = builder.AddParameter("pgPassword", secret: true);

var postgres = builder.AddPostgres("postgres", pgUsername, pgPassword, port: 15432)
    .WithImage("postgres")
    .WithDataVolume("withly-postgres-data");


var postgresDb = postgres.AddDatabase("WithlyDB", "WithlyDB");

builder.AddProject<Withly_API>("API")
    .WithReference(postgresDb, "DefaultConnection")
    .WaitFor(postgresDb);

builder.Build().Run();