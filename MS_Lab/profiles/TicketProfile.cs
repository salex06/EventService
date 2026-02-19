using AutoMapper;
using MS_Lab.dto.ticket;
using MS_Lab.entities;

namespace MS_Lab.profiles
{
    public class TicketProfile : Profile
    {
        public TicketProfile()
        {
            CreateMap<UpdateTicketDTO, Ticket>();
            //    .ForMember(
            //        dest => dest.Event.Id,
            //        opt => opt.Ignore()//MapFrom(src => src.EventId)
            //    )
            //    .ForMember(
            //        dest => dest.Owner,
            //        opt => opt.Ignore()//MapFrom(src => src.TicketOwner)
            //    )
            //    .ForMember(
            //        dest => dest.Owner.Id,
            //        opt => opt.Ignore() 
            //    )
            //    .ForAllMembers(opt => opt.Condition(
            //        (src, dest, srcMember) => srcMember != null 
            //    ));

            CreateMap<TicketDTO, Ticket>();
            CreateMap<Ticket, TicketDTO>();
            //.ForMember(
            //    dest => dest.TicketNumber,
            //    opt => opt.Ignore()//MapFrom(src => src.TicketNumber)
            //)
            //.ForMember(
            //    dest => dest.Id,
            //    opt => opt.Ignore()//MapFrom(src => src.Id)
            //)
            //.ForMember(
            //    dest => dest.EventId,
            //    opt => opt.Ignore()//MapFrom(src => src.Event.Id)
            //)
            //.ForMember(
            //    dest => dest.TicketOwner,
            //    opt => opt.Ignore()//MapFrom(src => src.Owner)
            //);

            CreateMap<TicketOwnerDTO, TicketOwner>();
                //.ForMember(
                //    dest => dest.Id,
                //    opt => opt.Ignore()
                //)
                //.ForMember(
                //    dest => dest.Name,
                //    opt => opt.Ignore()//MapFrom(src => src.Name)
                //)
                //.ForMember(
                //    dest => dest.Surname,
                //    opt => opt.Ignore()//MapFrom(src => src.Surname)
                //)
                //.ForMember(
                //    dest => dest.Phone,
                //    opt => opt.Ignore())//MapFrom(src => src.Phone))
                //.ForMember(
                //    dest => dest.Email,
                //    opt => opt.Ignore()//.MapFrom(src => src.Email)
                //);
            CreateMap<TicketOwner, TicketOwnerDTO>();

            CreateMap<CreateTicketDTO, Ticket>();
               //.ForMember(
               //    dest => dest.TicketNumber,
               //    opt => opt.Ignore()//MapFrom(src => GenerateTicketNumber())
               //)
               //.ForMember(
               //    dest => dest.PurchaseDate,
               //    opt => opt.Ignore()//MapFrom(src => DateTime.UtcNow)
               //)
               //.ForMember(
               //    dest => dest.Owner.Id,
               //    opt => opt.Ignore()//MapFrom(src =>
               //        //src.TicketOwner != null ? (int?)null : null)
               //)
               //.ForMember(
               //    dest => dest.Owner,
               //    opt => opt.Ignore()//MapFrom(src => src.TicketOwner) 
               //)
               //.ForMember(
               //    dest => dest.Id,
               //    opt => opt.Ignore() 
               //)
               //.ForMember(
               //    dest => dest.Event,
               //    opt => opt.Ignore() 
               //);
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
