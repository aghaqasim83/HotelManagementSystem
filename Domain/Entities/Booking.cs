using Domain.Common;

namespace Domain.Entities;

public class Booking : BaseEntity
{
    public string BookingReference { get; set; } = null!; // unique
    public string GuestName { get; set; } = null!;
    public required string RoomId { get; set; }
    public Room? Room { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
}