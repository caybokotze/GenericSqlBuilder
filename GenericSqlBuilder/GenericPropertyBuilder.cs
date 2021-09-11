using System;
using System.Data;
using System.Linq;

namespace GenericSqlBuilder
{
    public static class GenericPropertyBuilder<T> where T : class, new()
    {
        public static DataTable GetPropertiesFromType()
        {
            var type = typeof(T);
            return type.GetProperties()
                .Aggregate(
                    new DataTable(),
                    (acc, cur) =>
                    {
                        acc.Columns.Add(cur.Name,
                            Nullable.GetUnderlyingType(cur.PropertyType)
                            ?? cur.PropertyType);
                        return acc;
                    });
        }
    }
}