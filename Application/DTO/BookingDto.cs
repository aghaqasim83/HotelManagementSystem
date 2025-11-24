namespace Application.DTO;

public class BookingDto
{
    public string Id { get; set; } = null!;
    public string BookingReference { get; set; } = null!;
    public string GuestName { get; set; } = null!;
    public string RoomId { get; set; } = null!;
    public RoomDto? Room { get; set; }
    public string? RoomNumber { get; set; }
    public DateOnly CheckIn { get; set; }
    public DateOnly CheckOut { get; set; }
    public int NumberOfGuests { get; set; }
}
