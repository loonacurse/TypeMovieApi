using MongoDB.Bson;
using MongoDB.Driver;
using TelegramBotApi.Model1;

public class Constants
{
    public static string apikey = "17cc9e1720a9e88f5aa254294d22057a";
    public static string address = "https://api.themoviedb.org/3/";
    public static MongoClient mongoClient;
    public static IMongoDatabase database;
    public static IMongoCollection<BsonDocument> movie_list;
    public static IMongoCollection<BsonDocument> selected_list;
}

