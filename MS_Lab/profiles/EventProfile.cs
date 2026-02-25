using AutoMapper;
using Microsoft.Extensions.Logging;
using MS_Lab.dto.events;
using MS_Lab.entities;

namespace MS_Lab.profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<EventDto, Event>();
            CreateMap<Event, EventDto>();

            CreateMap<UpdateEventDto, Event>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateEventDto, Event>();

            CreateMap<Event, CreateEventDto>();
            CreateMap<CreateEventDto, Event>();
        }
    }
}
