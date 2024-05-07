using System;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simacek.Linq
{
    public static class SearchExtensions
    {
        private static readonly string Delimiter = ((char)007).ToString();
        private static readonly bool StopWordsEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableStopWords"]);
        private static readonly string[] StopWords = ConfigurationManager.AppSettings["StopWords"]?.Split(',');

        public static IQueryable<T> Search<T>(this IQueryable<T> source, string searchPhrase)
        {
            return Search(source, searchPhrase.ToSearchKeys());
        }

        private static IQueryable<T> Search<T>(this IQueryable<T> source, string[] keys)
        {
            if (keys == null || keys.Length < 1)
            {
                return source;
            }

            var type = typeof(T);
            var param = Expression.Parameter(type);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.PropertyType.IsValueType || x.PropertyType.IsPrimitive || x.PropertyType == typeof(string));

            Expression call = null;

            foreach (var _key in keys)
            {
                var not = _key.StartsWith("-");
                var key = not ? _key.Remove(0, 1) : _key;
                key = key.Replace(Delimiter, "");

                Expression call2 = null;
                foreach (var prop in properties)
                {
                    var toStr = prop.PropertyType.GetMethod("ToString", Type.EmptyTypes);
                    var contains = typeof(string).GetMethod("Contains");

                    var l = Expression.Property(param, prop);
                    var left = Expression.Call(l, toStr);
                    var right = Expression.Constant(key, typeof(string));

                    #region Nullable
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    {
                        var underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                        toStr = underlyingType.GetMethod("ToString", Type.EmptyTypes);
                        var c = Expression.Convert(l, underlyingType);
                        left = Expression.Call(c, toStr);
                    }
                    #endregion

                    #region DateTime
                    //This piece of logic allows proper searching of date times. 
                    //The search comparison converts the sql values to strings.
                    //The UI displays string dates differently than the sql string representation.
                    //This piece of logic converts the user UI input to match the sql representation.
                    if (prop.PropertyType == typeof(System.DateTime))
                    {
                        var datePieces = key.Split('/');
                        var dateStr = "";

                        var max = datePieces.Length == 3 ? 2 : datePieces.Length;

                        for (var i = 0; i < max; i++)
                        {
                            dateStr += (datePieces[i].Length == 1) ? $"0{datePieces[i]}" : datePieces[i];
                            if (i == 0)
                            {
                                dateStr += "-";
                            }
                        }
                        if (datePieces.Length == 3)
                        {
                            dateStr = $"{datePieces[2]}-{dateStr}";
                        }
                        right = string.IsNullOrWhiteSpace(dateStr) ? right : Expression.Constant(dateStr, typeof(string));
                    }
                    #endregion

                    Expression expr = Expression.Call(left, contains, right);
                    call2 = (call2 != null) ? Expression.OrElse(call2, expr) : expr;
                }

                call2 = not ? Expression.Not(call2) : call2;

                call = (call != null) ? Expression.And(call, call2) : call2;
            }

            var result = Expression.Call(typeof(Queryable), "Where", new[] { source.ElementType },
                source.Expression, Expression.Lambda<Func<T, bool>>(call, new[] { param }));

            return source.Provider.CreateQuery<T>(result);
        }

        private static string[] ToSearchKeys(this string source)
        {

            //remove extra quotation if there is an odd number
            var c = source.Count(x => x == '"');
            if (c % 2 != 0)
            {
                var i = source.LastIndexOf('"');
                source = source.Remove(i, 1);
            }

            var keys = source.Split('"')
                .Select((element, index) => (index % 2 == 0)
                    ? (element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => (x.StartsWith("-")) ? ($"-{Delimiter}{x.Remove(0, 1)}{Delimiter}") : x)) //Unquoted words
                    : new[] { $"{Delimiter}{string.Join(" ", element.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()))}{Delimiter}" }) //Quoted string cleaned of extra spaces
                .SelectMany(element => element.Where(x => x.Length > 0)).ToArray();

            var m = $"-{Delimiter}{Delimiter}";
            for (int i = 1; i < keys.Length; i++)
            {
                if (keys[i - 1] == m)
                {
                    keys[i] = $"-{keys[i]}";
                }
            }

            return keys.Where(x => x != m && x.Length > 0 && (!StopWordsEnabled || !StopWords.Contains(x, StringComparer.OrdinalIgnoreCase))).ToArray();
        }

    }
}
