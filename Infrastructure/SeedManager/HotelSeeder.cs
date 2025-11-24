using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.SeedManager;

public class HotelSeeder
{
    private readonly ICommandRepository<Hotel> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public HotelSeeder(
        ICommandRepository<Hotel> categoryRepository,
        IUnitOfWork unitOfWork
    )
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var customerCategories = new List<Hotel>
        {
            GetHotel("Premier Inn"),
            GetHotel("Holiday Inn")
        };

        foreach (var category in customerCategories)
        {
            await _categoryRepository.CreateAsync(category);
        }

        await _unitOfWork.SaveAsync();
    }

    private static Hotel GetHotel(string hotelName)
    {
        var hotel = new Hotel { Name = hotelName, Rooms = new List<Room>() };

        // 6 rooms seeded: 2 Single(1), 2 Double(2), 2 Deluxe(3)
        hotel.Rooms.Add(new Room { Number = "101", Type = RootTypeEnum.Single, Capacity = 1, HotelId = hotel.Id });
        hotel.Rooms.Add(new Room { Number = "102", Type = RootTypeEnum.Single, Capacity = 1, HotelId = hotel.Id });
        hotel.Rooms.Add(new Room { Number = "201", Type = RootTypeEnum.Double, Capacity = 2, HotelId = hotel.Id });
        hotel.Rooms.Add(new Room { Number = "202", Type = RootTypeEnum.Double, Capacity = 2, HotelId = hotel.Id });
        hotel.Rooms.Add(new Room { Number = "301", Type = RootTypeEnum.Deluxe, Capacity = 3, HotelId = hotel.Id });
        hotel.Rooms.Add(new Room { Number = "302", Type = RootTypeEnum.Deluxe, Capacity = 3, HotelId = hotel.Id });

        return hotel;
    }
}
