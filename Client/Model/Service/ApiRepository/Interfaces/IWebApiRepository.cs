using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Model.Entity;

namespace Client.Model.Service.ApiRepository.Interfaces
{
    public interface IWebApiRepository
    {
        Task<List<Hotel>> GetDataAsync(int pageSize = 10, int pageNumber = 1);
        Task<List<Hotel>> GetDataAsync();
        Task<List<Room>> GetDataAsync(string name);
        Task<Hotel> GetDataAsync(int id);
        Task<Hotel> PostDataAsync(Hotel myData);
        Task<Hotel> PutDataAsync(Hotel myData);
        Task<Hotel> DeleteDataAsync(int id);
    }
}
