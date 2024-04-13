using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Inferno_Cascade
{
    public static class Registry
    {
        private static Dictionary<object, object> _registry = new Dictionary<object, object>();

        public static void Register(object key, object value)
            => _registry.Add(key, value);

        public static void Deregister(object key)
            => _registry.Remove(key);

        public static TValue Get<TValue>(object key)
        {
            if (_registry.TryGetValue(key, out var value))
                return (TValue) value;
            return default;
        }
    }

    public static class RegistryStrings
    {
        public static string PlayerRigidbody = "PlayerRigidbody";
    }
}
