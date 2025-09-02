using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rfm.Api.Infrastructure;
using Rfm.Api.Models;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TourOperator> TourOperators => Set<TourOperator>();
    public DbSet<FlightRoute> Routes => Set<FlightRoute>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<BookingClass> BookingClasses => Set<BookingClass>();
    public DbSet<PriceEntry> PriceEntries => Set<PriceEntry>();

    // ✅ Add this line:
    public DbSet<TourOperatorSeason> TourOperatorSeasons => Set<TourOperatorSeason>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ✅ FlightRoute <-> BookingClass
        builder.Entity<FlightRoute>()
               .HasMany(fr => fr.BookingClasses)
               .WithMany(bc => bc.Routes);

        // ✅ TourOperator <-> Season
        builder.Entity<TourOperatorSeason>()
               .HasKey(tos => new { tos.TourOperatorId, tos.SeasonId });

        builder.Entity<TourOperatorSeason>()
       .HasOne(tos => tos.TourOperator)
       .WithMany(to => to.TourOperatorSeasons)
       .HasForeignKey(tos => tos.TourOperatorId)
       .OnDelete(DeleteBehavior.Cascade); 

        builder.Entity<TourOperatorSeason>()
            .HasOne(tos => tos.Season)
            .WithMany(s => s.TourOperatorSeasons)
            .HasForeignKey(tos => tos.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);  
    }
}
