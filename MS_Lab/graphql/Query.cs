using HotChocolate.Resolvers;
using MongoDB.Driver;
using MS_Lab.data;
using MS_Lab.dto.events;
using MS_Lab.dto.ticket;
using MS_Lab.entities;
using MS_Lab.exception;
using MS_Lab.services.events;
using MS_Lab.services.tickets;

namespace MS_Lab.graphql
{
    public class Query
    {
        public Task<IEnumerable<Event>> GetEvents([Service] IEventService svc)
        {
            EventFilterDto empty = new EventFilterDto();
            return svc.GetAllEventsAsync(empty);
        }

        public async Task<Event?> GetEventById(string id, [Service] IEventService svc)
        {
            try
            {
                return await svc.GetEventByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage(ex.Message)
                        .SetCode(ex.StatusCode.ToString())
                        .Build()
                );
            }
        }

        public Task<IEnumerable<Ticket>> GetTickets([Service] ITicketService svc)
        {
            TicketFilterDto empty = new TicketFilterDto();
            return svc.GetAllTicketsAsync(empty);
        }

        public async Task<Ticket?> GetTicketById(string id, [Service] ITicketService svc)
        {
            try
            {
                return await svc.GetTicketByIdAsync(id);
            }
            catch (NotFoundException ex)
            {
                throw new GraphQLException(
                    ErrorBuilder.New()
                        .SetMessage(ex.Message)
                        .SetCode(ex.StatusCode.ToString())
                        .Build()
                );
            }
        }
    }
}
