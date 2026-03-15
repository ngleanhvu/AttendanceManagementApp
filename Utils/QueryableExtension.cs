using System.Linq.Expressions;

namespace AttendanceManagementApp.Utils
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySearch<T>(
            this IQueryable<T> query,
            string search,
            params Expression<Func<T, string>>[] fields)
        {
            if (string.IsNullOrWhiteSpace(search) || fields.Length == 0)
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");

            Expression body = null;

            foreach (var field in fields)
            {
                var member = Expression.Invoke(field, parameter);

                var contains = Expression.Call(
                    member,
                    typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) }),
                    Expression.Constant(search)
                );

                body = body == null
                    ? contains
                    : Expression.OrElse(body, contains);
            }

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

            var property = Expression.PropertyOrField(param, sortBy);

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
