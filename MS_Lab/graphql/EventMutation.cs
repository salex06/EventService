using AutoMapper;
using Microsoft.Extensions.Options;
using MS_Lab.dto.events;
using MS_Lab.entities;
using MS_Lab.exception;
using MS_Lab.kafka.producer;
using MS_Lab.services.events;
using System.Reflection;

namespace MS_Lab.graphql
{
    public class EventMutation
    {
        private readonly IMapper _mapper;
        private readonly IKafkaMessageProducer _kafkaMessageProducer;
        private readonly ProducerSettings _producerSettings;

        public EventMutation(IMapper mapper, 
            IKafkaMessageProducer kafkaMessageProducer,
            IOptions<ProducerSettings> producerSettings)
        {
            _mapper = mapper;
            _kafkaMessageProducer = kafkaMessageProducer;
            _producerSettings = producerSettings.Value;
        }

        public async Task<Event> CreateEvent(CreateEventDto ev, [Service] IEventService svc) {
            var createdEvent = await svc.CreateEventAsync(_mapper.Map<Event>(ev));

            _kafkaMessageProducer.SendConfirmationRequest(_mapper.Map<EventDto>(createdEvent), 
                ev.ConfirmatorId, 
                _producerSettings.TopicName
            );

            return createdEvent;
        }

        public async Task<Event> UpdateEvent(string id, UpdateEventDto ev, [Service] IEventService svc) {
            try
            {
                return await svc.UpdateEventAsync(id, ev);
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

        public async Task<bool> DeleteEvent(string id, [Service] IEventService svc) {
            try
            {
                await svc.DeleteEventAsync(id);
                return true;
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
