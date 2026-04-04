namespace ClientService.dto
{
    public record RegObjectResponseDto
    {
        public int ObjType { get; set; }
        public string ObjId { get; set; } = "";
        public string ConfirmatorId { get; set; } = "";
        public DateTime ConfirmDateTime { get; set; }
    }
}
