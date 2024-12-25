using OpenSearch.Client;
using SearchEngine.API;
using Context = SearchEngine.API.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Register OpenSearch Client

IContext context = new Context(args[0]);

var settings = new ConnectionSettings(context.Configuration.Elastic.Uri)
    .BasicAuthentication(context.Configuration.Elastic.User, context.Configuration.Elastic.Password)
    .DefaultIndex(context.Configuration.Elastic.IndexName);  // Replace with your index name
        
var client = new OpenSearchClient(settings);

builder.Services.AddSingleton<IOpenSearchClient>(client);

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();