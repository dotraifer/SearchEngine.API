using Microsoft.OpenApi.Models;
using OpenSearch.Client;
using SearchEngine.API;
using Context = SearchEngine.API.Context;

var builder = WebApplication.CreateBuilder(args);

var context = new Context("config/config.yaml");

// Add services to the container
builder.Services.AddControllers();

// Register OpenSearch client
builder.Services.AddSingleton<IOpenSearchClient>(sp =>
{
    var settings = new ConnectionSettings(context.Configuration.Elastic.Uri) // Replace with your OpenSearch URL
        .BasicAuthentication(context.Configuration.Elastic.User, context.Configuration.Elastic.Password)
        .DefaultIndex(context.Configuration.Elastic.IndexName); // Replace with your default index
    return new OpenSearchClient(settings);
});

// Register Context class (if it's a custom class that holds configuration or other dependencies)
builder.Services.AddSingleton(context);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Search Engine API",
        Version = "v1",
        Description = "An API for searching documents using OpenSearch."
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Search Engine API V1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();