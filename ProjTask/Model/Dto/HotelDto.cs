namespace MinAPI.Model.Dto
{
    public class HotelDto
    {
        public int Id { get; set; }

        public string? HotelName { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public double? Rating { get; set; }

        public byte[]? Picture { get; set; }

        public List<RoomDto>? Rooms { get; set; }
    }
}
