using Domain.Common;

namespace Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; } = null!;
    public List<Room> Rooms { get; set; } = new();
}
