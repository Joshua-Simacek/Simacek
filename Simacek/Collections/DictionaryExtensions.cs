using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Simacek.Collections
{
    public static partial class DictionaryExtensions
    {
        public static SqlParameter[] ToSqlParameters(this Dictionary<string, object> source)
        {
            return source.Select(param => new SqlParameter(param.Key, param.Value)).ToArray();
        }

    }
}
