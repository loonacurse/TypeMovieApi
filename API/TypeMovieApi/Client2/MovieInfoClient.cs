using Newtonsoft.Json;
using TelegramBotApi.Model1;

namespace TelegramBotApi.Client2
{
    public class MovieInfoClient
    {
        private HttpClient _httpClient;
        private static string _address;
        private static string _apikey;
        public MovieInfoClient()
        {
            _address = Constants.address;
            _apikey = Constants.apikey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<Models> GetMovieInfoAsync(string name)
        {
            var response = await _httpClient.GetAsync($"search/movie?api_key=17cc9e1720a9e88f5aa254294d22057a&language=uk-UA&query={name}&page=1&include_adult=false");
            response.EnsureSuccessStatusCode();
            var content = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<Models>(content);
            return result;

        }
    }
}
