using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Model.Service.Auth
{
    public class Authorization
    {
        public string? _username { private get; set; }
        public string? _password { private get; set; }
        private readonly HttpClient _httpClient;
        private readonly string? URL_LOGIN = string.Empty;
        public Authorization(HttpClient client)
        {
            _httpClient = client;
            //_httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiUrl"]);
            _username = ConfigurationManager.AppSettings["Name"];
            _password = ConfigurationManager.AppSettings["Pass"];
            URL_LOGIN = $"{ConfigurationManager.AppSettings["ApiUrl"]}login?username={_username}&password={_password}";
        }
        public async Task<string> GetAuthToken()=>
            await _httpClient.GetAsync(URL_LOGIN).Result.Content.ReadAsStringAsync();

    }
}
