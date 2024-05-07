using System;
using System.Collections.Generic;
using System.Data;

namespace Simacek.Collections
{
    public static partial class IEnumerableExtensions
    {
        public static IEnumerable<T> Add<T>(this IEnumerable<T> source, T addItem)
        {
            foreach (var item in source)
            {
                yield return item;
            }
            yield return addItem;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var dataTable = new DataTable();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var propertyType = prop.PropertyType.IsGenericType
                    ? prop.PropertyType.GetGenericArguments()[0]
                    : prop.PropertyType;

                var dataColumn = new DataColumn(prop.Name, propertyType);

                if (prop.CanRead)
                {
                    dataTable.Columns.Add(dataColumn);
                }
            }

            foreach (var item in source)
            {
                var dataRow = dataTable.NewRow();

                var count = dataTable.Columns.Count;
                for (var prop = 0; prop < count; prop++)
                {
                    if (properties[prop].CanRead)
                    {
                        dataRow[prop] = properties[prop].GetValue(item, null);
                    }
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
    }
}
