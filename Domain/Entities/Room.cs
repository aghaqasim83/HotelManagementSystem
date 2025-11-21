using Domain.Common;

namespace Domain.Entities;

public class Room : BaseEntity
{
    public string Number { get; set; } = null!; // e.g., "101"
    public string Type { get; set; } = null!; // "Single", "Double", "Deluxe"
    public int Capacity { get; set; }

    public required string HotelId { get; set; }
    public Hotel? Hotel { get; set; }

    public List<Booking> Bookings { get; set; } = new();
}