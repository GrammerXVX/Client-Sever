using System;
using System.Collections.Generic;

namespace MinAPI.Model.Entity;

public partial class Hotel
{
    public int Id { get; set; }

    public string? HotelName { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public double? Rating { get; set; }

    public byte[]? Picture { get; set; }

    public virtual ICollection<HotelRoom> HotelRooms { get; } = new List<HotelRoom>();
}

