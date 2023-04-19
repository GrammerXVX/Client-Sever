using System;
using System.Collections.Generic;

namespace MinAPI.Model.Entity;

public partial class HotelRoom
{
    public int Id { get; set; }

    public int? HotelId { get; set; }

    public int? RoomId { get; set; }

    public double? Price { get; set; }

    public virtual Hotel? Hotel { get; set; }

    public virtual Room? Room { get; set; }
}
