using MISA.Fresher.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Misa.Fresher.Infrastructure.Helpers
{
    public class DbColumnMapper
    {
        private static readonly Dictionary<Type, Dictionary<string, string>> _cache = new();

        public static string GetColumnName<T>(string propertyName)
        {
            var type = typeof(T);

            if (!_cache.TryGetValue(type, out var map))
            {
                map = type.GetProperties()
                    .Select(p => new
                    {
                        Property = p.Name,
                        Column = p.GetCustomAttribute<DbColumnAttribute>()?.Name
                    })
                    .Where(x => x.Column != null)
                    .ToDictionary(x => x.Property, x => x.Column!);

                _cache[type] = map;
            }

            return map.TryGetValue(propertyName, out var column)
                ? column
                : throw new InvalidOperationException(
                    $"Property '{propertyName}' not mapped to DB column.");
        }
    }
}
