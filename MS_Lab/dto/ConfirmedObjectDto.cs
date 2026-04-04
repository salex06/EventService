namespace MS_Lab.dto
{
    public record ConfirmedObjectDto
    {
        public ObjectType ObjType { get; set; }
        public string ObjId { get; set; } = "";
        public string ConfirmatorId { get; set; } = "";
        public DateTime ConfirmDateTime { get; set; }
    }
}
