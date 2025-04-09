var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);
var sql = builder.AddSqlServer("sql", password, 1433);
    //.WithDataVolume();

var db = sql.AddDatabase("MuseumsDb");
builder.AddProject<Projects.IntegrationTestsOnContainers_Web>("integrationtestsoncontainers-web")
    .WithReference(db)
    .WaitFor(db);

builder.Build().Run();
