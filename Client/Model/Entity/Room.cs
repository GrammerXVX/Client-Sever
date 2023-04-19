namespace Client.Model.Entity;

public partial record Room
{
    public int Id { get; set; }
    public string? Number { get; set; }
    public string? RoomType { get; set; }
    public double? Price{ get; set; }

}
