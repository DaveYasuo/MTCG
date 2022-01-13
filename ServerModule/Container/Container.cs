using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServerModule.Exceptions;

namespace ServerModule.Container
{
    // How to inherit XML Comments from Interface for example
    // See: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/xmldoc/recommended-tags#inheritdoc
    /// <inheritdoc cref="IContainer"/>
    public class Container : IContainer
    {
        // Generic methods for adding instances to the container
        // See: https://stackoverflow.com/a/15717047
        private readonly Dictionary<Type, Func<object>> _storage = new();

        public void Register<TService, TImpl>() where TImpl : TService =>
            _storage.Add(typeof(TService), () => GetInstance<TImpl>());

        public void Register<TService>(Func<TService> factory) =>
            _storage.Add(typeof(TService), () => factory());

        public void RegisterInstance<TService>(TService instance) =>
            _storage.Add(typeof(TService), () => instance);

        public void RegisterSingleton<TService>(Func<TService> factory)
        {
            // lazy for thread safety
            // See: https://riptutorial.com/csharp/example/6795/
            Lazy<TService> lazy = new Lazy<TService>(factory);
            Register(() => lazy.Value);
        }

        public void RegisterSingleton<TService>()
        {
            TService instance = (TService)GetInstance<TService>();
            TService Factory() => instance;
            // lazy for thread safety
            // See: https://riptutorial.com/csharp/example/6795/
            Lazy<TService> lazy = new Lazy<TService>(Factory);
            Register(() => lazy.Value);
        }

        public T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        /// Check if Instance is already registered which can be created or is already created and returns the instance.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ContainerException"></exception>
        /// <returns>The Instance with the given type</returns>
        private object GetInstance(Type type)
        {
            if (_storage.TryGetValue(type, out Func<object> fac)) return fac();
            if (!type.IsAbstract) return CreateInstance(type);
            throw new ContainerException("No registration for " + type);
        }

        /// <summary>
        /// Creates a new Instance every time GetInstance get invoked, excepts Singleton
        /// </summary>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        private object CreateInstance(Type implementationType)
        {
            // Only supports one ctor
            ConstructorInfo ctor = implementationType.GetConstructors().Single();
            IEnumerable<Type> paramTypes = ctor.GetParameters().Select(p => p.ParameterType);
            object[] dependencies = paramTypes.Select(GetInstance).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}