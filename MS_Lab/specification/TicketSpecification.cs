using LinqKit;
using MS_Lab.dto.events;
using MS_Lab.dto.ticket;
using MS_Lab.entities;

namespace MS_Lab.specification
{
    public static class TicketSpecification
    {
        public static ISpecification<Ticket>? FromFilter(TicketFilterDto filter)
        {
            if (filter == null || !filter.HasFilters())
            {
                return null;
            }

            var predicate = PredicateBuilder.New<Ticket>();

            if (!string.IsNullOrWhiteSpace(filter.TicketNumber))
            {
                predicate.And(p => p.TicketNumber.Contains(filter.TicketNumber));
            }

            if (filter.MinPurchaseDate.HasValue)
            {
                predicate.And(p => p.PurchaseDate >= filter.MinPurchaseDate);
            }

            if (filter.MaxPurchaseDate.HasValue)
            {
                predicate.And(p => p.PurchaseDate <= filter.MaxPurchaseDate);
            }

            return new Specification<Ticket>(predicate);
        }
    }
}
