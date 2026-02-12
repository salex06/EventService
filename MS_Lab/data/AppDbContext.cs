using Microsoft.EntityFrameworkCore;
using MS_Lab.entities;

namespace MS_Lab.data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketOwner> TicketOwners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasIndex(e => e.StartTimeUTC)
                .HasDatabaseName("ix_events_start_time");

            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique()
                .HasDatabaseName("ix_tickets_number");

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Owner)
                .WithOne(o => o.Ticket)
                .HasForeignKey<Ticket>(t => t.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TicketOwner>()
                .HasIndex(o => o.Email)
                .HasDatabaseName("ix_ticket_owners_email");
        }
    }
}