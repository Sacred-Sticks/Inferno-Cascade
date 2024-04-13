using Kickstarter.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Kickstarter.DependencyInjection
{
    [DefaultExecutionOrder(-1000)]
    public class Injector : Singleton<Injector>
    {
        const BindingFlags _bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        readonly Dictionary<Type, object> registry = new Dictionary<Type, object>();

        protected override void Awake()
        {
            base.Awake();

            // Find all modules implementing IDependencyProvider
            var providers = FindMonoBehaviours().OfType<IDependencyProvider>();
            foreach (var provider in providers)
                RegisterProvider(provider);

            // Find all injectable object and inject their dependencies
            var injectables = FindMonoBehaviours().Where(IsInjectable);
            foreach (var injectable in injectables)
                Inject(injectable);
        }

        private static bool IsInjectable(MonoBehaviour obj)
        {
            var members = obj.GetType().GetMembers(_bindingFlags);
            return members.Any(member => Attribute.IsDefined(member, typeof(InjectAttribute)));
        }

        private static MonoBehaviour[] FindMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }

        private void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(_bindingFlags);
            RegisterMethodProviders(provider, methods);
            var fields = provider.GetType().GetFields(_bindingFlags);
            RegisterFieldProviders(provider, fields);
            var properties = provider.GetType().GetProperties(_bindingFlags);
            RegisterPropertyProviders(provider, properties);

            void RegisterMethodProviders(IDependencyProvider provider, MethodInfo[] methods)
            {
                foreach (var method in methods)
                {
                    if (!Attribute.IsDefined(method, typeof(ProvideAttribute)))
                        continue;

                    var returnType = method.ReturnType;
                    var providedInstance = method.Invoke(provider, null);
                    if (providedInstance == null)
                        throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
                    registry.Add(returnType, providedInstance);
                }
            }

            void RegisterFieldProviders(IDependencyProvider provider, FieldInfo[] fields)
            {
                foreach (var field in fields)
                {
                    if (!Attribute.IsDefined(field, typeof(ProvideAttribute)))
                        continue;

                    var returnType = field.FieldType;
                    var providedInstance = field.GetValue(provider);
                    if (providedInstance == null)
                        throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
                    registry.Add(returnType, providedInstance);
                }
            }

            void RegisterPropertyProviders(IDependencyProvider provider, PropertyInfo[] properties)
            {
                foreach (var property in properties)
                {
                    if (!Attribute.IsDefined(property, typeof(ProvideAttribute)))
                        continue;

                    var returnType = property.PropertyType;
                    var providedInstance = property.GetValue(provider);
                    if (providedInstance == null)
                        throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
                    registry.Add(returnType, providedInstance);
                }
            }
        }

        private object Resolve(Type type)
        {
            registry.TryGetValue(type, out var resolvedInstance);
            return resolvedInstance;
        }

        private void Inject(object instance)
        {
            var type = instance.GetType();

            InjectFields(instance, type);
            InjectMethods(instance, type);
            InjectProperties(instance, type);

            void InjectFields(object instance, Type type)
            {
                var injectableFields = type.GetFields(_bindingFlags)
                    .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

                foreach (var injectableField in injectableFields)
                {
                    var fieldType = injectableField.FieldType;
                    var resolvedInstance = Resolve(fieldType);
                    if (resolvedInstance == null)
                        throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}");

                    injectableField.SetValue(instance, resolvedInstance);
                }
            }

            void InjectMethods(object instance, Type type)
            {
                var injectableMethods = type.GetMethods(_bindingFlags)
                    .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

                foreach (var injectableMethod in injectableMethods)
                {
                    var requiredParamaters = injectableMethod.GetParameters()
                        .Select(parameter => parameter.ParameterType)
                        .ToArray();
                    var resolvedInstances = requiredParamaters
                        .Select(Resolve)
                        .ToArray();
                    if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                        throw new Exception($"Failed to resolve parameters for {type.Name}.{injectableMethod.Name}");

                    injectableMethod.Invoke(instance, resolvedInstances);
                }
            }

            void InjectProperties(object instance, Type type)
            {
                var injectableProperties = type.GetProperties(_bindingFlags)
                    .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

                foreach (var injectableProperty in injectableProperties)
                {
                    var fieldType = injectableProperty.PropertyType;
                    var resolvedInstance = Resolve(fieldType);
                    if (resolvedInstance == null)
                        throw new Exception($"Failed to resolve {fieldType.Name} for {type.Name}");

                    injectableProperty.SetValue(instance, resolvedInstance);
                }
            }
        }
    }
}
