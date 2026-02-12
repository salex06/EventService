using AutoMapper;
using MS_Lab.dto.ticket;
using MS_Lab.entities;

namespace MS_Lab.profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<UpdateTicketDTO, Ticket>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<TicketDTO, Ticket>();
            CreateMap<Ticket, TicketDTO>();

            CreateMap<TicketOwnerDTO, TicketOwner>();
            CreateMap<TicketOwner, TicketOwnerDTO>();
        }
    }
}
