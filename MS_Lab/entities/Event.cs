using MS_Lab.enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Lab.entities
{

    [Table("events")]
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("place")]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Column("event_type")]
        public EventType EventType { get; set; }  // enum как int

        [Required]
        [Column("start_time_utc")]
        public DateTime StartTimeUTC { get; set; }

        [Required]
        [Column("end_time_utc")]
        public DateTime EndTimeUTC { get; set; }

        [Required]
        [Column("ticket_count")]
        public int TicketCount { get; set; }

        [Required]
        [Column("price")]
        public int Price { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }

}