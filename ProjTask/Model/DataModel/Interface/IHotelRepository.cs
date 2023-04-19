using MinAPI.Model.Dto;
using MinAPI.Model.Entity;

namespace MinAPI.Model.DataModel.Interface
{
    public interface IHotelRepository : IDisposable
    {
        Task<List<Hotel>> GetHotelsAsync();
        Task<List<Hotel>> GetHotelsAsync(int pageSize,int pageNumber);
        Task<List<Hotel>> GetHotelsAsync(string name);
        Task<List<RoomDto>> GetHotelWithRooms(string hotelName);
        Task<Hotel> GetHotelAsync(int Id);
        Task InsertHotelAsync(Hotel hotel);
        Task UpdateHotelAsync(HotelDto hotel);
        Task DeleteHotelAsync(int Id);
        Task SaveAsync();
    }
}
