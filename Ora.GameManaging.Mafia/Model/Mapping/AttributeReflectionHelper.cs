using System.Reflection;
using Ora.GameManaging.Mafia.Data;

namespace Ora.GameManaging.Mafia.Model.Mapping
{
    public static class AttributeReflectionHelper
    {
        public static void ApplyAttributesToModel<TModel>(TModel model, IEnumerable<GeneralAttributeEntity> attributes)
        {
            var type = typeof(TModel);
            foreach (var attr in attributes)
            {
                var prop = type.GetProperty(attr.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null || !prop.CanWrite)
                    continue;

                object? value = null;
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                try
                {
                    if (propType == typeof(string))
                        value = attr.Value;
                    else if (propType == typeof(int))
                        value = int.TryParse(attr.Value, out var intVal) ? intVal : null;
                    else if (propType == typeof(bool))
                        value = bool.TryParse(attr.Value, out var boolVal) ? boolVal : null;
                    else if (propType == typeof(DateTime))
                        value = DateTime.TryParse(attr.Value, out var dateVal) ? dateVal : null;
                    // Add more types if needed
                }
                catch
                {
                    // Ignore conversion errors
                }

                if (value != null)
                    prop.SetValue(model, value);
            }
        }
    }
}