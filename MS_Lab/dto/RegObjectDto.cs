namespace MS_Lab.dto
{
    public record RegObjectDto
    {
        public ObjectType Type { get; set; }
        public string ObjectId { get; set; } = "";
        public string ConfirmatorId { get; set; } = "";
    }

    public enum ObjectType { 
        Event,
        Ticket
    }
}
