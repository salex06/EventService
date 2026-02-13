using MS_Lab.entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MS_Lab.entities
{
    [Table("tickets")]
    public class Ticket
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("event_id")]
        public int EventId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("ticket_number")]
        public string TicketNumber { get; set; } = Guid.NewGuid().ToString();

        [Column("owner_id")]
        public int? OwnerId { get; set; }

        [Required]
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("EventId")]
        public Event Event { get; set; } = null!;

        [ForeignKey("OwnerId")]
        public TicketOwner? Owner { get; set; }
    }
}