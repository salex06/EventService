using LinqKit;
using MS_Lab.dto.events;
using MS_Lab.entities;
using System.Linq.Expressions;

namespace MS_Lab.specification
{
    public static class EventSpecification
    {
        public static ISpecification<Event>? FromFilter(EventFilterDto filter) {
            if (filter == null || !filter.HasFilters()) {
                return null;
            }

            var predicate = PredicateBuilder.New<Event>();

            if (!string.IsNullOrWhiteSpace(filter.Name)) { 
                predicate.And(p => p.Name.Contains(filter.Name));
            }

            if (!string.IsNullOrWhiteSpace(filter.Description))
            {
                predicate.And(p => p.Description != null 
                    && p.Description.Contains(filter.Description));
            }

            if (!string.IsNullOrWhiteSpace(filter.Place))
            {
                predicate.And(p => p.Place.Contains(filter.Place));
            }

            if (filter.EventType.HasValue) {
                predicate.And(p => p.EventType == filter.EventType);
            }

            if (filter.MinStartTimeUTC.HasValue) {
                predicate.And(p => p.StartTimeUTC >= filter.MinStartTimeUTC);
            }

            if (filter.MaxStartTimeUTC.HasValue)
            {
                predicate.And(p => p.StartTimeUTC <= filter.MaxStartTimeUTC);
            }

            if (filter.MinEndTimeUTC.HasValue) {
                predicate.And(p => p.EndTimeUTC >= filter.MinEndTimeUTC);
            }

            if (filter.MaxEndTimeUTC.HasValue)
            {
                predicate.And(p => p.EndTimeUTC <= filter.MaxEndTimeUTC);
            }

            if (filter.MinPrice.HasValue) {
                predicate.And(p => p.Price >= filter.MinPrice);
            }

            if (filter.MaxPrice.HasValue) {
                predicate.And(p => p.Price <= filter.MaxPrice);
            }

            if (filter.MinTicketCount.HasValue) {
                predicate.And(p => p.TicketCount >= filter.MinTicketCount);
            }

            if (filter.MaxTicketCount.HasValue) {
                predicate.And(p => p.TicketCount <= filter.MaxTicketCount);
            }

            return new Specification<Event>(predicate);
        }
    }
}
