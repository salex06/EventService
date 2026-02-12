using AutoMapper;
using Microsoft.Extensions.Logging;
using MS_Lab.dto.events;
using MS_Lab.entities;

namespace MS_Lab.profiles
{
    public class EventProfile : Profile 
    {
        public EventProfile() {
            CreateMap<EventDTO, Event>();
            CreateMap<Event, EventDTO>();

            CreateMap<Event, UpdateEventDTO>();
            CreateMap<UpdateEventDTO, Event>();

            CreateMap<Event, CreateEventDTO>();
            CreateMap<CreateEventDTO, Event>();
        }
    }
}
