using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Simacek.Linq
{
    public static class OrderbyExtensions
    {
        #region IQueryable
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IList<KeyValuePair<string, Order>> sortParameters)
        {
            if (sortParameters == null)
            {
                throw new ArgumentNullException(nameof(sortParameters));
            }

            if (sortParameters.Count < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(sortParameters));
            }

            var type = typeof(T);
            var param = Expression.Parameter(type);
            MethodCallExpression result = null;

            foreach (var sortParameter in sortParameters)
            {
                var prop = type.GetProperty(sortParameter.Key);
                var propAccess = Expression.MakeMemberAccess(param, prop);
                var condition = Expression.Lambda(propAccess, param);
                var orderBy = (result != null) ? (sortParameter.Value != Order.Ascending ? "ThenByDescending" : "ThenBy") : (sortParameter.Value != Order.Ascending ? "OrderByDescending" : "OrderBy");
                result = Expression.Call(typeof(Queryable), orderBy, new[] { type, prop.PropertyType }, result ?? source.Expression, Expression.Quote(condition));
            }

            return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(result);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, IList<KeyValuePair<string, string>> sortParameters)
        {
            return OrderBy<T>(source, MapSortOrders(sortParameters));
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, Dictionary<string, string> sortParameters)
        {
            return OrderBy(source, sortParameters?.ToList());
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, Dictionary<string, Order> sortParameters)
        {
            return OrderBy<T>(source, sortParameters.ToList());
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, Order sortOrder = Order.Ascending)
        {
            return OrderBy<T>(source, new List<KeyValuePair<string, Order>>() { new KeyValuePair<string, Order>(orderByProperty, sortOrder) });
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, string sortOrder)
        {
            return OrderBy<T>(source, new List<KeyValuePair<string, Order>>() { new KeyValuePair<string, Order>(orderByProperty, GetSortOrder(sortOrder)) });
        }
        #endregion


        #region IEnumerable
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IList<KeyValuePair<string, Order>> sortParameters)
        {
            return (IOrderedEnumerable<T>)OrderBy(source.AsQueryable(), sortParameters);
        }
        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IList<KeyValuePair<string, string>> sortParameters)
        {
            return (IOrderedEnumerable<T>)OrderBy<T>(source.AsQueryable(), MapSortOrders(sortParameters));
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Dictionary<string, string> sortParameters)
        {
            return (IOrderedEnumerable<T>)OrderBy<T>(source.AsQueryable(), sortParameters.ToList());
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Dictionary<string, Order> sortParameters)
        {
            return (IOrderedEnumerable<T>)OrderBy<T>(source.AsQueryable(), sortParameters.ToList());
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string orderByProperty, Order sortOrder = Order.Ascending)
        {
            return (IOrderedEnumerable<T>)OrderBy<T>(source.AsQueryable(), new List<KeyValuePair<string, Order>>() { new KeyValuePair<string, Order>(orderByProperty, sortOrder) });
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string orderByProperty, string sortOrder)
        {
            return (IOrderedEnumerable<T>)OrderBy<T>(source.AsQueryable(), new List<KeyValuePair<string, Order>>() { new KeyValuePair<string, Order>(orderByProperty, GetSortOrder(sortOrder)) });
        }
        #endregion


        #region private functions
        private static Order GetSortOrder(string sortOrder)
        {
            var asc = new[] { "asc", "ascend", "ascending", };
            var desc = new[] { "desc", "descend", "descending" };

            Order order = (Order)(-1);
            var validSort = false;

            if (asc.Contains(sortOrder.ToLower()))
            {
                order = Order.Ascending;
                validSort = true;
            }

            if (desc.Contains(sortOrder.ToLower()))
            {
                order = Order.Descending;
                validSort = true;
            }

            if (!validSort)
            {
                throw new ArgumentException($"[{sortOrder}] is not a valid SortOrder", nameof(sortOrder));
            }

            return order;
        }

        private static IList<KeyValuePair<string, Order>> MapSortOrders(IList<KeyValuePair<string, string>> sortParameters)
        {
            return sortParameters.Select(x => new KeyValuePair<string, Order>(x.Key, GetSortOrder(x.Value))).ToList();
        }
        #endregion

    }
}
