using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Simacek.Collections
{
    public static partial class NameValueCollectionExtensions
    {
        public static IDictionary<string, object> ToDictionary(this NameValueCollection source)
        {
            return source.Keys.Cast<string>().ToDictionary<string, string, object>(key => key, key => source[key]);
        }
    }
}
