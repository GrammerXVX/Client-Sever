using Client.Model.Service.ApiRepository.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Client.Model.Service.Auth;
using Client.Model.Entity;
using System.Windows;

namespace Client.Model.Service.ApiRepository
{
    public class WebApiRepository : IWebApiRepository
    {
        private readonly HttpClient _httpClient;
        private readonly string? _accessToken;

        public WebApiRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            
            _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiUrl"]);
            try
            {
                _accessToken ??=  new Authorization(_httpClient).GetAuthToken().Result.Trim('"');
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}\nThe server is not responding or is unavailable. Try the following steps to resolve the issue.\r\n1)Check your internet connection.\r\n2)Start the \"Server MinAPI\" server from the folder with the client.\r\n3)Contact the software vendor.\r\n4)Try to connect File/Reconnect.", "Server is not available", MessageBoxButton.OK, MessageBoxImage.Error) ;
                _httpClient.Dispose();
            }            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
        }

        public async Task<Hotel> GetDataAsync(int id) =>
           await DeserializeResponse<Hotel>(_httpClient.GetAsync($"hotels/{id}").Result);
        public async Task<List<Room>> GetDataAsync(string name) =>
            await DeserializeResponse<List<Room>>(_httpClient.GetAsync($"hotels/name/{name}").Result);
        public async Task<List<Hotel>> GetDataAsync(int pageSize, int pageNumber) =>
            await DeserializeResponse<List<Hotel>>(_httpClient.GetAsync($"hotels/{pageSize}/{pageNumber}").Result);
        public async Task<List<Hotel>> GetDataAsync() =>
            await DeserializeResponse<List<Hotel>>(_httpClient.GetAsync($"hotels").Result);

        public async Task<Hotel> PostDataAsync(Hotel hotel)
        {
            var json = SerializeObject(hotel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("hotels", content);
            return await DeserializeResponse<Hotel>(response);
        }

        public async Task<Hotel> PutDataAsync(Hotel hotel)
        {
            var json = SerializeObject(hotel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("hotels", content);
            return await DeserializeResponse<Hotel>(response);
        }

        public async Task<Hotel> DeleteDataAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"hotels/{id}");
            return await DeserializeResponse<Hotel>(response);
        }

        private static async Task<T> DeserializeResponse<T>(HttpResponseMessage response) =>
            JsonConvert.DeserializeObject<T>(value: await response.Content.ReadAsStringAsync());

        private static string SerializeObject(object obj) =>
            JsonConvert.SerializeObject(obj);
    }
}
