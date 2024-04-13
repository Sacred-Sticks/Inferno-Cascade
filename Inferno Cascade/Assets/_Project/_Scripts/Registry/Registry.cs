using System.Collections.Generic;

namespace Inferno_Cascade
{
    public static class Registry
    {
        private static Dictionary<object, object> _registry = new Dictionary<object, object>();

        public static void Register(object key, object value)
        {
            _registry.Add(key, value);
        }

        public static void Deregister(object key)
        {
            _registry.Remove(key);
        }

        public static T Get<T>(object key)
        {
            return (T) _registry[key];
        }
    }

    public static class RegistryStrings
    {
        public static string PlayerRigidbody = "PlayerRigidbody";
    }
}
