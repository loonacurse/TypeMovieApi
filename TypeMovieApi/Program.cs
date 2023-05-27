using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using TelegramBotApi.Model1;

Constants.mongoClient = new MongoClient("mongodb+srv://loonacurse:04(Darinka)10@typemovie.n4kn7xo.mongodb.net/");
Constants.database = Constants.mongoClient.GetDatabase("typemovie");
Constants.movie_list = Constants.database.GetCollection<BsonDocument>("movie_list");
Constants.selected_list = Constants.database.GetCollection<BsonDocument>("selected_list");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// äîáàâëåíèå Swagger
builder.Services.AddSwaggerGen(options =>
{
    // çàäàíèå ïàðàìåòðîâ Swagger
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });
});

var app = builder.Build();

// íàñòðîéêà Swagger
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