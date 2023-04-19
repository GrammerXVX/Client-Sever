using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MinAPI.Model;
using MinAPI.Model.DataModel.Interface;
using MinAPI.Model.Dto;
using MinAPI.Model.Entity;

namespace MinAPI.Model.DataModel
{
    public class HotelRepository : IHotelRepository
    {
        private readonly HotelDb _context;
        //private readonly IMapper _mapper;
        public HotelRepository(HotelDb context) =>
            _context = context;

        public async Task<List<Hotel>> GetHotelsAsync( int pageSize, int pageNumber)=>
            await _context.Hotels.Skip(((pageNumber<=0?1: pageNumber) - 1) * pageSize).Take(pageSize <= 0 ? 1 : pageSize).ToListAsync();
        public async Task<List<Hotel>> GetHotelsAsync()=>
            await _context.Hotels.ToListAsync();
        public async Task<List<Hotel>> GetHotelsAsync(string name) =>
            await _context.Hotels.Where(h => h.HotelName.Contains(name)).ToListAsync();
        public async Task<List<RoomDto>> GetHotelWithRooms(string hotelName) =>
            await _context.RoomDto.FromSql(sql: $"SELECT * FROM dbo.GetHotelWithRooms({GetHotelsAsync(hotelName).Result.FirstOrDefault().HotelName})").ToListAsync();
        public async Task<Hotel> GetHotelAsync(int Id) =>
            await _context.Hotels.FindAsync(new object[] { Id });

        public async Task InsertHotelAsync(Hotel hotel) => await _context.Hotels.AddAsync(hotel);

        public async Task UpdateHotelAsync(HotelDto hotel)
        {
            var hotelFromDb = await _context.Hotels.FindAsync(new object[] { hotel.Id });

            if (hotelFromDb == null) return;
            hotelFromDb.HotelName = hotel.HotelName;
            hotelFromDb.Address = hotel.Address;
            hotelFromDb.Phone = hotel.Phone;
            hotelFromDb.Picture = hotel.Picture;
            hotelFromDb.Rating = hotel.Rating;
            _context.Hotels.Update(hotelFromDb);
            if (hotel.Rooms != null)
                foreach (var x in hotel.Rooms)
                {
                    if (await _context.Rooms.FindAsync(new object[] { x.Id }) == null && await _context.Rooms.FirstOrDefaultAsync(y => y.Number == x.Number && y.Type == _context.RoomTypes.FirstOrDefault(y => y.Name == x.RoomType).Id) == null)
                    {
                        await _context.Rooms.AddAsync(new Room() { Number = x.Number, Type = _context.RoomTypes.FirstAsync(y => y.Name == x.RoomType).Result.Id });
                        await _context.SaveChangesAsync();

                    }
                    if (await _context.HotelRooms.FirstOrDefaultAsync(y => y.HotelId == hotel.Id && y.RoomId == x.Id) == null)
                    {
                        var roomType = await _context.RoomTypes.FirstOrDefaultAsync(y => y.Name == x.RoomType);
                        var room = await _context.Rooms.FirstOrDefaultAsync(y => y.Number == x.Number && y.Type == roomType.Id);
                        await _context.HotelRooms.AddAsync(new HotelRoom()
                        {
                            HotelId = hotel.Id,
                            RoomId = room.Id,
                            Price = x.Price,
                        });
                    }
                }
        }

        public async Task DeleteHotelAsync(int Id)
        {
            var hotelFromDb = await _context.Hotels.FindAsync(new object[] { Id });
            if (hotelFromDb == null) return;
            _context.Hotels.Remove(hotelFromDb);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


    }
}
