using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Dal
{
    public sealed class EntityDbContext : IdentityDbContext<User>
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntityDbContext(DbContextOptions<EntityDbContext> optionsBuilderOptions) : base(optionsBuilderOptions)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Item> Items { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Schedule> Schedules { get; set; }
    }
}