namespace MinAPI.Model.Entity;

public partial class Room
{
    public int Id { get; set; }

    public string? Number { get; set; }

    public int? Type { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<HotelRoom> HotelRooms { get; } = new List<HotelRoom>();

    public virtual RoomType? TypeNavigation { get; set; }
}
