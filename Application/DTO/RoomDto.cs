namespace Application.DTO;

public class RoomDto
{
    public string Id { get; set; } = null!;
    public string Number { get; set; } = null!;
    public int Capacity { get; set; }
    public required string Type { get; set; }
    public string HotelId { get; set; } = null!;
}
