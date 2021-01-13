using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicQuerying
{
    public static class DynamicQuerying
    {
        public static class QueryType
        {
            public static string Equal { get; } = "equal";
            public static string NotEqual { get; } = "notequal";
            public static string Contains { get; } = "contains";
            public static string GreaterThan { get; } = "greaterthan";
            public static string GreaterThanOrEqual { get; } = "greaterthanorequal";
            public static string LessThan { get; } = "lessthan";
            public static string LessThanOrEqual { get; } = "lessthanorequal";
            public static string OrderBy { get; } = "orderby";
            public static string OrderByDescending { get; } = "orderbydescending";
        }
        public static IQueryable<TElement> RunQueries<TElement>(this IQueryable<TElement> collection,IEnumerable<Query> queries)
        {
            var entityType = typeof(TElement);
            var queryedCollection = collection;

            var parameterExpression = Expression.Parameter(entityType);
            Expression rootExpression = null;

            foreach(var query in queries)
            {
                query.Type = query.Type.ToLower();
            }

            collection = collection.OrderBy(queries.Where(x => x.Type == QueryType.OrderBy ||x.Type== QueryType.OrderByDescending));

            foreach (var query in queries.Where(x => !string.IsNullOrWhiteSpace(x.Value)&&x.Type!=QueryType.OrderBy && x.Type!=QueryType.OrderByDescending))
            {
                var property = Expression.Property(parameterExpression, query.Field);

                var propertyType = ((PropertyInfo)property.Member).PropertyType;
                Expression expression;
                if (query.Type == QueryType.Contains)
                {
                    var indexOf = Expression.Call(property, "IndexOf", null, Expression.Constant(query.Value, typeof(string)), Expression.Constant(StringComparison.InvariantCultureIgnoreCase));
                    expression = Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(propertyType); // 1

                    if (!converter.CanConvertFrom(typeof(string))) // 2
                        throw new NotSupportedException();

                    var propertyValue = converter.ConvertFromInvariantString(query.Value); // 3
                    var constant = Expression.Constant(propertyValue);
                    var valueExpression = Expression.Convert(constant, propertyType); // 4
                    if ( query.Type == QueryType.GreaterThan)
                    {
                        expression = Expression.GreaterThan(property, constant);
                    }
                    else if (query.Type == QueryType.GreaterThanOrEqual)
                    {
                        expression = Expression.GreaterThanOrEqual(property, constant);
                    }
                    else if (query.Type == QueryType.LessThan)
                    {
                        expression = Expression.LessThan(property, constant);
                    }
                    else if (query.Type == QueryType.LessThanOrEqual)
                    {
                        expression = Expression.LessThanOrEqual(property, constant);
                    }
                    else if (query.Type == QueryType.NotEqual)
                    {
                        expression = Expression.NotEqual(property, constant);
                    }
                    else
                    {
                        expression = Expression.Equal(property, constant);
                    }

                }
                if (rootExpression == null)
                    rootExpression = expression;
                else
                    rootExpression = Expression.And(expression, rootExpression);
            }
            if (rootExpression == null)
                return collection;
            var final = Expression.Lambda<Func<TElement, bool>>(body: rootExpression, parameters: parameterExpression).Compile();

            return collection.Where(final).AsQueryable();
        }

        private static IQueryable<TElement> OrderBy<TElement>(
           this IQueryable<TElement> collection,
           IEnumerable<Query> queries)
        {
            // Basically sortedColumns contains the columns user wants to sort by, and 
            // the sorting direction.
            // For my screenshot, the sortedColumns looks like
            // [
            //     { "cassette", { Order = 1, Direction = SortDirection.Ascending } },
            //     { "slotNumber", { Order = 2, Direction = SortDirection.Ascending } }
            // ]

            bool firstTime = true;

            // The type that represents each row in the table
            var itemType = typeof(TElement);

            // Name the parameter passed into the lamda "x", of the type TModel
            var parameter = Expression.Parameter(itemType, "x");

            // Loop through the sorted columns to build the expression tree
            foreach (var query in queries)
            {
                // Get the property from the TModel, based on the key
                var prop = Expression.Property(parameter, query.Field);

                // Build something like x => x.Cassette or x => x.SlotNumber
                var exp = Expression.Lambda(prop, parameter);

                // Based on the sorting direction, get the right method
                string method = String.Empty;
                if (firstTime)
                {
                    method = query.Type == QueryType.OrderBy
                        ? "OrderBy"
                        : "OrderByDescending";

                    firstTime = false;
                }
                else
                {
                    method = query.Type == QueryType.OrderBy
                        ? "ThenBy"
                        : "ThenByDescending";
                }

                // itemType is the type of the TModel
                // exp.Body.Type is the type of the property. Again, for Cassette, it's
                //     a String. For SlotNumber, it's a Double.
                Type[] types = new Type[] { itemType, exp.Body.Type };

                // Build the call expression
                // It will look something like:
                //     OrderBy*(x => x.Cassette) or Order*(x => x.SlotNumber)
                //     ThenBy*(x => x.Cassette) or ThenBy*(x => x.SlotNumber)
                var mce = Expression.Call(typeof(Queryable), method, types,
                    collection.Expression, exp);

                // Now you can run the expression against the collection
                collection = collection.Provider.CreateQuery<TElement>(mce);
            }

            return collection;
        }
    }
    public class Query
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Field { get; set; }
    }
}
