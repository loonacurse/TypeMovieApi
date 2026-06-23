using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using TelegramBotApi.Model1;

Constants.mongoClient = new MongoClient("mongodb+srv://04darinka10_db_user:2HqltNkjssWIGkbY@typemoviedb.2wbrptk.mongodb.net/");
Constants.database = Constants.mongoClient.GetDatabase("typemovie");
Constants.movie_list = Constants.database.GetCollection<BsonDocument>("movie_list");
Constants.selected_list = Constants.database.GetCollection<BsonDocument>("selected_list");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();