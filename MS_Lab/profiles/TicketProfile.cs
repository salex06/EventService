using AutoMapper;
using MS_Lab.dto.ticket;
using MS_Lab.entities;

namespace MS_Lab.profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<Ticket, TicketDTO>()
             .ForMember(dest => dest.EventId,
                 opt => opt.MapFrom(src => src.Event.Id))
             .ForMember(dest => dest.TicketOwner,
                 opt => opt.MapFrom(src => src.Owner));
            CreateMap<TicketOwner, TicketOwnerDTO>();

            CreateMap<CreateTicketDTO, Ticket>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForMember(dest => dest.TicketNumber,
                opt => opt.MapFrom(src => GenerateTicketNumber()))
            .ForMember(dest => dest.PurchaseDate,
                opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Event,
                opt => opt.MapFrom(src => new Event { Id = src.EventId }))
            .ForMember(dest => dest.Owner,
                opt => opt.MapFrom(src => src.TicketOwner));


            CreateMap<TicketOwnerDTO, TicketOwner>()
             .ForMember(dest => dest.Id,
                 opt => opt.Ignore())
             .ForMember(dest => dest.Name,
                 opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Surname,
                 opt => opt.MapFrom(src => src.Surname))
             .ForMember(dest => dest.Phone,
                 opt => opt.MapFrom(src => src.Phone))
             .ForMember(dest => dest.Email,
                 opt => opt.MapFrom(src => src.Email));

            CreateMap<UpdateTicketDTO, Ticket>()
               .ForMember(dest => dest.Owner,
                   opt => opt.MapFrom(src => src.TicketOwner))
               .ForMember(dest => dest.Id,
                   opt => opt.Ignore()) 
               .ForMember(dest => dest.Event,
                   opt => opt.Ignore()) 
               .ForMember(dest => dest.TicketNumber,
                   opt => opt.Ignore()) 
               .ForMember(dest => dest.PurchaseDate,
                   opt => opt.Ignore());

            CreateMap<TicketOwnerDTO, TicketOwner>()
            .ForMember(dest => dest.Id,
                opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                srcMember != null));
        }

        private string GenerateTicketNumber()
        {
            // Формат: TICKET-YYYYMMDD-XXXXXXXX (X - случайные символы)
            var date = DateTime.UtcNow.ToString("yyyyMMdd");
            var random = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            return $"TICKET-{date}-{random}";
        }
    }
}
