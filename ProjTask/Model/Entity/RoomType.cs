using System;
using System.Collections.Generic;

namespace MinAPI.Model.Entity;

public partial class RoomType
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Room> Rooms { get; } = new List<Room>();
}
