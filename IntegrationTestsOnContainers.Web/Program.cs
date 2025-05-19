using System.Reflection;
using IntegrationTestsOnContainers.ServiceDefaults;
using IntegrationTestsOnContainers.Web;
using IntegrationTestsOnContainers.Web.Database;
using Scalar.AspNetCore; 
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.AddSqlServerDbContext<ApplicationDbContext>(connectionName: "MuseumsDb");
var app = builder.Build();

app.MapDefaultEndpoints();
app.InitializeDb();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(_ =>
    {
        _.Servers = [];
        _.Theme = ScalarTheme.DeepSpace;
    });

}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 

app.Run();

namespace IntegrationTestsOnContainers.Web
{
    public partial class Program { }
}