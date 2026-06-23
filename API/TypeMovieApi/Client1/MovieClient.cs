using TelegramBotApi.Model1;
using Newtonsoft.Json;

namespace TelegramBotApi.Client1
{
    public class MovieClient
    {
        private HttpClient _httpClient;
        private static string _address;
        private static string _apikey;
        public MovieClient()
        {
            _address = Constants.address;
            _apikey = Constants.apikey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        public async Task<Models> GetMovieNowPlayingAsync()
        {
            var response1 = await _httpClient.GetAsync($"movie/now_playing?api_key=17cc9e1720a9e88f5aa254294d22057a&language=uk-UA&page=1&region=UA");
            response1.EnsureSuccessStatusCode();
            var content1 = response1.Content.ReadAsStringAsync().Result;
            var result1 = JsonConvert.DeserializeObject<Models>(content1);
            return result1;
        }
    }
}
