using AutoMapper;
using MS_Lab.dto.ticket;

namespace MS_Lab.profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<TicketDTO, Ticket>();
            CreateMap<Ticket, TicketDTO>();

            CreateMap<TicketOwnerDTO, TicketOwner>();
            CreateMap<TicketOwner, TicketOwnerDTO>();
        }
    }
}
