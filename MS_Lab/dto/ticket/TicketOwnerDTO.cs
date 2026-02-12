namespace MS_Lab.dto.ticket
{
    public record TicketOwnerDTO
    {
        public int Id;
        public string Name;
        public string Surname;

        public string Phone;
        public string Email;

        public TicketOwnerDTO(int id, string name, string surname, string phone, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Phone = phone;
            Email = email;
        }
    }
}
