using System.Linq.Expressions;

namespace AttendanceManagementApp.Utils
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySearch<T>(
            this IQueryable<T> query,
            string search,
            Expression<Func<T, string>> field)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            var parameter = field.Parameters[0];

            var body = Expression.Call(
                field.Body,
                typeof(string).GetMethod("Contains", new[] { typeof(string) }),
                Expression.Constant(search)
            );

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

            return query.Where(predicate);
        }

        public static IQueryable<T> ApplyPagination<T>(
            this IQueryable<T> query,
            int page,
            int pageSize)
        {
            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }

        public static IQueryable<T> ApplySorting<T>(
            this IQueryable<T> query,
            string sortBy,
            bool desc)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query;

            var param = Expression.Parameter(typeof(T), "x");

            var property = Expression.Property(param, sortBy);

            var lambda = Expression.Lambda(property, param);

            string method = desc ? "OrderByDescending" : "OrderBy";

            var result = Expression.Call(
                typeof(Queryable),
                method,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(lambda)
            );

            return query.Provider.CreateQuery<T>(result);
        }
    }
}
