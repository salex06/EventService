using System.Linq.Expressions;

namespace MS_Lab.specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
    }

    public class Specification<T> : ISpecification<T>
    {
        public Specification(Expression<Func<T, bool>> criteria) {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>> Criteria { get; }
    }
}
