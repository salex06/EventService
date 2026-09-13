using HotChocolate.Resolvers;
using MongoDB.Driver;
using MS_Lab.data;
using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.exception;
using MS_Lab.services.events;

namespace MS_Lab.graphql
{
    public class EventQuery
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
    }
}
