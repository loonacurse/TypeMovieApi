using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using TelegramBotApi.Client1;
using TelegramBotApi.Client2;
using TelegramBotApi.Model1;

namespace TelegramBotApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesController : ControllerBase
    {
        public static List<Selected_Array> GettingBsonDoc_selected(long id)
        {
            try
            {
                var general_selected_Array = new BsonArray();
                var general_selected_list = new List<Selected_Array>();
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                BsonDocument _updatedDocument = Constants.selected_list.Find(filter).FirstOrDefault();
                if (_updatedDocument == null)
                {
                    return general_selected_list;
                }
                var selectedArray = _updatedDocument["selected_Array"].AsBsonArray;
                for (int i = 0; i < selectedArray.Count; i++)
                {
                    var got_selected_movie = selectedArray.AsBsonValue[i].ToString();
                    var converted_selected_movie = JsonConvert.DeserializeObject<Selected_Array>(got_selected_movie);
                    general_selected_list.Add(converted_selected_movie);
                }
                return general_selected_list;
            }
           catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        public static List<Movie> GettingBsonDoc_watched(long id)
        {
            try
            {
                var general_watched_Array = new BsonArray();
                var general_watched_list = new List<Movie>();
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                BsonDocument __updatedDocument = Constants.movie_list.Find(filter).FirstOrDefault();
                if (__updatedDocument == null)
                {
                    return general_watched_list;
                }
                var watchedArray = __updatedDocument["watched_Array"].AsBsonArray;
                for (int i = 0; i < watchedArray.Count; i++)
                {
                    var get_watched_movie = watchedArray.AsBsonArray[i].ToJson();
                    var converted_watch_movie = JsonConvert.DeserializeObject<Movie>(get_watched_movie);
                    general_watched_list.Add(converted_watch_movie);
                }
                return general_watched_list;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        private readonly ILogger<MoviesController> logger;
        public MoviesController(ILogger<MoviesController> logger)
        {
            this.logger = logger;
        }
        [HttpGet("cinema_movie")]
        public Models Movies()
        {
            try
            {
                MovieClient client = new MovieClient();
                return client.GetMovieNowPlayingAsync().Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        [HttpGet("get_movie_by_person")]
        public MovieActorModel ActorMovies(string actor_name)
        {
            try
            {
                GetActorMovieClient client3 = new GetActorMovieClient();
                return client3.GetActorMovieAsync(actor_name).Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        [HttpGet("movie_info")]
        public Models GetMovieInfo(string name)
        {
            try
            {
                MovieInfoClient client2 = new MovieInfoClient();
                return client2.GetMovieInfoAsync(name).Result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }

        }
        [HttpGet("get_watched_list")]
         public List<Movie> GetWatchedList(long id)
        {
            try
            {
                return GettingBsonDoc_watched(id);
            }
            catch
            {
                throw new Exception();
                Console.WriteLine(new Exception());
            }
        }
        [HttpPost("add_watched_movie")]
        public bool CreateMovieList(Movie addmovie, long id)
        {
            try
            {
                var general_watched_Array = new BsonArray();
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var watched_exists = Constants.movie_list.Find(filter).Any();
                var Watched_Document = new BsonDocument
            {
                {"movie_name", addmovie.Movie_name},
                {"movie_rate", addmovie.Movie_rate },
                {"movie_comment", addmovie.Movie_comment }
            };
                var watched_list_document = new BsonDocument
                {
                {"user_id", id},
                {"watched_Array", new BsonArray(general_watched_Array) }
                };

                if (!watched_exists)
                {
                    Constants.movie_list.InsertOne(watched_list_document);
                    general_watched_Array = new BsonArray
                {
                    new BsonDocument(Watched_Document)
                };
                }
                else if (watched_exists)
                {
                    BsonDocument __updatedDocument = Constants.movie_list.Find(filter).FirstOrDefault();
                    var watchedArray = __updatedDocument["watched_Array"].AsBsonArray;
                    for (int i = 0; i < watchedArray.Count; i++)
                    {
                        var get_watched_movie = watchedArray.AsBsonArray[i].ToJson();
                        var converted_watch_movie = JsonConvert.DeserializeObject<Movie>(get_watched_movie);
                        var converted_doc = new BsonDocument
                        {
                            {"movie_name", converted_watch_movie.Movie_name},
                            {"movie_rate", converted_watch_movie.Movie_rate},
                            {"movie_comment", converted_watch_movie.Movie_comment},
                        };
                        general_watched_Array.Add(converted_doc);
                    }
                    general_watched_Array.Add(Watched_Document);
                }
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("watched_Array", new BsonArray(general_watched_Array));
                UpdateResult result = Constants.movie_list.UpdateOne(filter, update);
                UpdateDefinition<BsonDocument> updatedDocument = Constants.movie_list.Find(filter).FirstOrDefault();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }
        [HttpPut("update_watched_movie_name")]
        public bool UpdateWatchedMovie_Name(string movie_name, string new_name, long id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var general_watched_list = GettingBsonDoc_watched(id);              
                var new_general_watched_list = new List<Movie>();
                var Count = 0;
                for (int i = 0; i < general_watched_list.Count; i++)
                {
                    if (movie_name == general_watched_list[i].Movie_name && Count==0)
                    {
                        general_watched_list[i].Movie_name = new_name;
                        Count++;
                    }
                    new_general_watched_list.Add(general_watched_list[i]);
                }
                if (Count == 0)
                {
                    throw new Exception("item doesn`t exist");
                }
                var general_watched_Array_upd = new BsonArray();
                foreach (var m in new_general_watched_list)
                {
                    var Watched_Document = new BsonDocument
                                {
                                    {"movie_name", m.Movie_name},
                                    {"movie_rate", m.Movie_rate},
                                    {"movie_comment", m.Movie_comment},
                                };
                    general_watched_Array_upd.Add(Watched_Document);
                }
                var watched_list_document = new BsonDocument
                        {
                            { "user_id", id},
                            {"watched_Array", new BsonArray(general_watched_Array_upd) }
                        };
                Constants.movie_list.FindOneAndReplace(filter, watched_list_document);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        [HttpPut("update_watched_movie_rate")]
        public bool UpdateWatchedMovie_Rate(string movie_name, int new_rate, long id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var general_watched_list = GettingBsonDoc_watched(id);              
                var new_general_watched_list = new List<Movie>();
                var Count = 0;
                for (int i = 0; i < general_watched_list.Count; i++)
                {
                    if (movie_name == general_watched_list[i].Movie_name && Count == 0)
                    {
                        general_watched_list[i].Movie_rate = new_rate;
                        Count++;
                    }
                    new_general_watched_list.Add(general_watched_list[i]);
                }
                if (Count == 0)
                {
                    throw new Exception("item doesn`t exist");
                }
                var general_watched_Array_upd = new BsonArray();
                foreach (var m in new_general_watched_list)
                {
                    var Watched_Document = new BsonDocument
                                {
                                    {"movie_name", m.Movie_name},
                                    {"movie_rate", m.Movie_rate},
                                    {"movie_comment", m.Movie_comment},
                                };
                    general_watched_Array_upd.Add(Watched_Document);
                }
                var watched_list_document = new BsonDocument
                        {
                            { "user_id", id},
                            {"watched_Array", new BsonArray(general_watched_Array_upd) }
                        };
                Constants.movie_list.FindOneAndReplace(filter, watched_list_document);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }        
        [HttpPut("update_watched_movie_comment")]
        public bool UpdateWatchedMovie_Comment(string movie_name, string new_comment, long id)
        {
                try
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                    var general_watched_list = GettingBsonDoc_watched(id);                   
                    var new_general_watched_list = new List<Movie>();
                    var Count = 0;
                    for (int i = 0; i < general_watched_list.Count; i++)
                    {
                        if (movie_name == general_watched_list[i].Movie_name && Count == 0)
                        {
                            general_watched_list[i].Movie_comment = new_comment;
                            Count++;
                        }
                        new_general_watched_list.Add(general_watched_list[i]);
                    }
                    if (Count == 0)
                    {
                        throw new Exception("item doesn`t exist");
                    }
                    var general_watched_Array_upd = new BsonArray();
                    foreach (var m in new_general_watched_list)
                    {
                        var Watched_Document = new BsonDocument
                                {
                                    {"movie_name", m.Movie_name},
                                    {"movie_rate", m.Movie_rate},
                                    {"movie_comment", m.Movie_comment},
                                };
                        general_watched_Array_upd.Add(Watched_Document);
                    }
                    var watched_list_document = new BsonDocument
                        {
                            { "user_id", id},
                            {"watched_Array", new BsonArray(general_watched_Array_upd) }
                        };
                    Constants.movie_list.FindOneAndReplace(filter, watched_list_document);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return false;
                }            
        }
        [HttpDelete("delete_watched_movie")]
        public bool DeleteWatchedList(string movie_name, long id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var general_watched_list = GettingBsonDoc_watched(id);
                var Count = 0;
                for (int i = 0; i < general_watched_list.Count; i++)
                {
                        if (movie_name == general_watched_list[i].Movie_name)
                        {
                            general_watched_list.Remove(general_watched_list[i]);
                            Count++;
                            break;
                        }
                }
                if (Count == 0)
                {
                    throw new Exception("item doesn`t exist");
                }
                var general_watched_Array_upd = new BsonArray();
                foreach (var m in general_watched_list)
                {
                    var Welected_Document = new BsonDocument
                            {
                                {"movie_name", m.Movie_name},
                                {"movie_rate", m.Movie_rate},
                                {"movie_comment", m.Movie_comment},
                            };
                    general_watched_Array_upd.Add(Welected_Document);
                }
                var watched_list_document = new BsonDocument
                        {
                            { "user_id", id},
                            {"watched_Array", new BsonArray(general_watched_Array_upd) }
                        };
                Constants.movie_list.FindOneAndReplace(filter, watched_list_document);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }        
        [HttpGet("get_selected_list")]
        public List<Selected_Array> GetSelectedList(long id)
        {
            try
            {               
                return GettingBsonDoc_selected(id);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        [HttpPost("add_selected_movie")]
        public bool PostSelectedList(Selected_Array addmovie, long id)
        {
            try
            {
                var general_selected_Array = new BsonArray();
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var selected_exists = Constants.selected_list.Find(filter).Any();
                var Selected_Document = new BsonDocument
                {
                    {"movie_name", addmovie.Movie_name}
                };
                var selected_list_document = new BsonDocument
                {
                { "user_id", id},
                {"selected_Array", new BsonArray(general_selected_Array) }
                };
                if (!selected_exists)
                {
                    Constants.selected_list.InsertOne(selected_list_document);
                    general_selected_Array = new BsonArray
                {
                    new BsonDocument(Selected_Document)
                };
                }
                else if (selected_exists)
                {
                    BsonDocument _updatedDocument = Constants.selected_list.Find(filter).FirstOrDefault();
                    var selectedArray = _updatedDocument["selected_Array"].AsBsonArray;
                    for (int i = 0; i < selectedArray.Count; i++)
                    {
                        var got_selected_movie = selectedArray.AsBsonValue[i].ToString();
                        var converted_selected_movie = JsonConvert.DeserializeObject<Selected_Array>(got_selected_movie);
                        var converted_doc = new BsonDocument
                        {
                            {"movie_name", converted_selected_movie.Movie_name}
                        };
                        general_selected_Array.Add(converted_doc);
                    }
                    general_selected_Array.Add(Selected_Document);
                }
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Set("selected_Array", new BsonArray(general_selected_Array));
                UpdateResult result = Constants.selected_list.UpdateOne(filter, update);
                return true;            
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }
        [HttpPut("update_selected_movie")]
        public bool UpdateSelectedList(string movie_name, string new_name, long id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var general_selected_list = GettingBsonDoc_selected(id);
                var new_general_selected_list = new List<Selected_Array>();
                var Count = 0;
                for (int i = 0; i < general_selected_list.Count; i++)
                {
                    if (movie_name == general_selected_list[i].Movie_name && Count == 0)
                    {
                        general_selected_list[i].Movie_name = new_name;
                        Count++;
                    }
                    new_general_selected_list.Add(general_selected_list[i]);
                }
                if (Count == 0)
                {
                    throw new Exception("item doesn`t exist");
                }
                var general_selected_Array_upd = new BsonArray();
                foreach (var m in new_general_selected_list)
                {
                    var Selected_Document = new BsonDocument
                                {
                                    {"movie_name", m.Movie_name},
                                };
                    general_selected_Array_upd.Add(Selected_Document);
                }
                var selected_list_document = new BsonDocument
                        {
                            { "user_id", id},
                            {"selected_Array", new BsonArray(general_selected_Array_upd) }
                        };
                Constants.selected_list.FindOneAndReplace(filter, selected_list_document);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
                return false;
            }
        }
        [HttpDelete("delete_selected_movie")]
        public bool DeleteSelectedMovie(string movie_name, long id)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.Eq("user_id", id);
                var general_selected_list = GettingBsonDoc_selected(id);
                var Count = 0;
                for (int i = 0; i < general_selected_list.Count; i++)
                {
                    if (movie_name == general_selected_list[i].Movie_name)
                    {
                        general_selected_list.Remove(general_selected_list[i]);
                        Count++;
                        break;
                    }
                }
                if (Count == 0)
                {
                    throw new Exception("item doesn`t exist");
                }
                var general_selected_Array_upd = new BsonArray();
                foreach (var m in general_selected_list)
                {
                    var Selected_Document = new BsonDocument
                                {
                                    {"movie_name", m.Movie_name},
                                };
                    general_selected_Array_upd.Add(Selected_Document);
                }
                var selected_list_document = new BsonDocument
                        {
                            { "user_id",id},
                            {"selected_Array", new BsonArray(general_selected_Array_upd) }
                        };
                Constants.selected_list.FindOneAndReplace(filter, selected_list_document);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
