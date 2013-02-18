using System.ComponentModel;
using System.Web.Http.OData.Query;
using NHibernate;

namespace System.Web.Http.OData.NHibernate
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ODataQueryOptionExtensions
    {
        public static IQuery ApplyTo(this ODataQueryOptions query, ISession session)
        {
            string from = "from " + query.Context.ElementClrType.Name + " $it" + Environment.NewLine;

            // convert $filter to HQL where clause.
            string where = ToString(query.Filter);

            // convert $orderby to HQL orderby clause.
            string orderBy = ToString(query.OrderBy);

            // create a query using the where clause and the orderby clause.
            string queryString = from + where + orderBy;
            IQuery hQuery = session.CreateQuery(queryString);

            // Apply $skip.
            hQuery = hQuery.Apply(query.Skip);

            // Apply $top.
            hQuery = hQuery.Apply(query.Top);

            return hQuery;
        }

        private static IQuery Apply(this IQuery query, TopQueryOption topQuery)
        {
            if (topQuery != null)
            {
                query = query.SetMaxResults(topQuery.Value);
            }

            return query;
        }

        private static IQuery Apply(this IQuery query, SkipQueryOption skipQuery)
        {
            if (skipQuery != null)
            {
                query = query.SetFirstResult(skipQuery.Value);
            }

            return query;
        }

        private static string ToString(OrderByQueryOption orderByQuery)
        {
            return NHibernateOrderByBinder.BindOrderByQueryOption(orderByQuery);
        }

        private static string ToString(FilterQueryOption filterQuery)
        {
            return NHibernateFilterBinder.BindFilterQueryOption(filterQuery);
        }
    }
}
