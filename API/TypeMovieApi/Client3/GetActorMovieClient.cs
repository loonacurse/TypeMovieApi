using Newtonsoft.Json;
using System.Reflection;
using TelegramBotApi.Model1;

namespace TelegramBotApi
{
    public class GetActorMovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        private static string _apikey;
        public GetActorMovieClient()
        {
            _address = Constants.address;
            _apikey = Constants.apikey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<MovieActorModel> GetActorMovieAsync(string actor_name)
        {
            var response = await _httpClient.GetAsync($"search/person?api_key=17cc9e1720a9e88f5aa254294d22057a&language=uk-UA&query={actor_name}&page=1&include_adult=false");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<MovieActorModel>(content);
            return result;

        }
    }
}
