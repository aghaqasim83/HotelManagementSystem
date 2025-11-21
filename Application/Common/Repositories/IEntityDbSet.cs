using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Repositories;

public interface IEntityDbSet
{
    public DbSet<Hotel> Hotel { get; set; }
}
