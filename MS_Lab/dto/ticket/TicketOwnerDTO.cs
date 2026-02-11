namespace MS_Lab.dto.ticket
{
    public record TicketOwnerDTO
    {
        int Id;
        string Name;
        string Surname;

        string Phone;
        string Email;

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
