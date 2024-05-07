using System;

namespace Simacek.Reflection
{
    public static partial class ReflectionExtensions
    {
        public static bool HasProperty(this Type model, string property)
        {
            return model.GetProperty(property) != null;
        }
    }
}
