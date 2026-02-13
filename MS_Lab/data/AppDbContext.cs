using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),  
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) 
            );

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events");

                entity.HasKey(e => e.Id)
                      .HasName("PK_events");

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .HasColumnType("bigint")
                      .ValueGeneratedOnAdd()
                      .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasColumnType("character varying(200)")
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.Description)
                      .HasColumnName("description")
                      .HasColumnType("text");

                entity.Property(e => e.Place)
                      .HasColumnName("place")
                      .HasColumnType("character varying(200)")
                      .HasMaxLength(200)
                      .IsRequired();

                entity.Property(e => e.EventType)
                      .HasColumnName("event_type")
                      .HasColumnType("integer")
                      .IsRequired();

                entity.Property(e => e.StartTimeUTC)
                      .HasColumnName("start_time_utc")
                      .HasColumnType("timestamp with time zone")
                      .HasConversion(dateTimeConverter)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.Property(e => e.EndTimeUTC)
                      .HasColumnName("end_time_utc")
                      .HasConversion(dateTimeConverter)
                      .HasColumnType("timestamp with time zone")
                      .IsRequired();

                entity.Property(e => e.TicketCount)
                      .HasColumnName("ticket_count")
                      .HasColumnType("bigint")
                      .IsRequired();

                entity.Property(e => e.Price)
                      .HasColumnName("price")
                      .HasColumnType("bigint")
                      .IsRequired();

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasColumnType("timestamp with time zone")
                      .HasConversion(dateTimeConverter)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasIndex(e => e.EventType)
                      .HasDatabaseName("IX_events_event_type");

                entity.HasIndex(e => e.StartTimeUTC)
                      .HasDatabaseName("IX_events_start_time");
            });

            // Конфигурация для TicketOwner
            modelBuilder.Entity<TicketOwner>(entity =>
            {
                entity.ToTable("ticket_owners");

                entity.HasKey(e => e.Id)
                      .HasName("PK_ticket_owners");

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .HasColumnType("bigint")
                      .ValueGeneratedOnAdd()
                      .UseIdentityAlwaysColumn();

                entity.Property(e => e.Name)
                      .HasColumnName("name")
                      .HasColumnType("character varying(100)")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Surname)
                      .HasColumnName("surname")
                      .HasColumnType("character varying(100)")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(e => e.Phone)
                      .HasColumnName("phone")
                      .HasColumnType("character varying(20)")
                      .HasMaxLength(20);

                entity.Property(e => e.Email)
                      .HasColumnName("email")
                      .HasColumnType("character varying(100)")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.HasIndex(e => e.Email)
                      .HasDatabaseName("IX_ticket_owners_email");

                entity.HasIndex(e => new { e.Surname, e.Name })
                      .HasDatabaseName("IX_ticket_owners_name");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("tickets");

                entity.HasKey(e => e.Id)
                      .HasName("PK_tickets");

                entity.Property(e => e.Id)
                      .HasColumnName("id")
                      .HasColumnType("bigint")
                      .ValueGeneratedOnAdd()
                      .UseIdentityAlwaysColumn();

                entity.Property(e => e.TicketNumber)
                      .HasColumnName("ticket_number")
                      .HasColumnType("character varying(50)")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.HasIndex(e => e.TicketNumber)
                      .IsUnique()
                      .HasDatabaseName("IX_tickets_ticket_number");

                entity.Property(e => e.PurchaseDate)
                      .HasColumnName("purchase_date")
                      .HasConversion(dateTimeConverter)
                      .HasColumnType("timestamp with time zone")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP")
                      .IsRequired();

                entity.Property(e => e.EventId)
                      .HasColumnName("event_id")
                      .HasColumnType("bigint");

                entity.Property(e => e.OwnerId)
                      .HasColumnName("owner_id")
                      .HasColumnType("bigint");

                entity.HasOne(e => e.Event)
                      .WithMany(e => e.Tickets)
                      .HasForeignKey(e => e.EventId)
                      .HasConstraintName("FK_tickets_events_event_id")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Owner)
                      .WithOne(o => o.Ticket)
                      .HasConstraintName("FK_tickets_ticket_owners_owner_id")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.EventId)
                      .HasDatabaseName("IX_tickets_event_id");

                entity.HasIndex(e => e.OwnerId)
                      .HasDatabaseName("IX_tickets_owner_id");
            });
        }
    }
}