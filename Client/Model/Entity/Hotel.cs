using System.Collections.Generic;

namespace Client.Model.Entity;
public partial class Hotel
{
    public int Id { get; set; }

    public string? HotelName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public double? Rating { get; set; }

    public byte[]? Picture { get; set; }

    public List<Room>? Rooms { get; set; }
    public override string ToString()=>
        $"Hotel name: {HotelName}\nPhone: {Phone}\nAddress: {Address}\nRating: {Rating}";
}

