using System;
using System.Collections.Generic;
using System.Linq;
using ServerModule.Exceptions;

namespace ServerModule.Utility
{
    public static class DependencyService
    {
        // Generic methods for adding instances to the container/dependency service
        // See: https://stackoverflow.com/a/15717047
        // Lock object for thread safety
        private static readonly object DependencyLock = new();
        private static readonly Dictionary<Type, Func<object>> Storage = new();

        /// <summary>
        ///     TService is the Interface, TImpl is the Class which implements the TService
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        public static void Register<TService, TImpl>() where TImpl : TService
        {
            Add(typeof(TService), () => GetInstance<TImpl>());
        }

        /// <summary>
        ///     Use this method when using the factory design pattern
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        public static void Register<TService>(Func<TService> factory)
        {
            Add(typeof(TService), () => factory());
        }

        /// <summary>
        ///     Use this method like "new Class(paramsCanBeEmpty)"
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        public static void RegisterInstance<TService>(TService instance)
        {
            Add(typeof(TService), () => instance);
        }

        /// <summary>
        ///     Use this method when using the factory design pattern and only one Instance is wanted
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        public static void RegisterSingleton<TService>(Func<TService> factory)
        {
            // lazy for thread safety
            // See: https://riptutorial.com/csharp/example/6795/
            var lazy = new Lazy<TService>(factory);
            Register(() => lazy.Value);
        }

        /// <summary>
        ///     Use this method, when only one instance is needed, but it depends on other Classes
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public static void RegisterSingleton<TService>()
        {
            var instance = GetInstance<TService>();

            TService Factory()
            {
                return instance;
            }

            // lazy for thread safety
            // See: https://riptutorial.com/csharp/example/6795/
            var lazy = new Lazy<TService>(Factory);
            Register(() => lazy.Value);
        }

        /// <summary>
        ///     Gets the specific Instance, which can be Singleton or a new Instance. It depends on how it was registered.
        /// </summary>
        /// <returns>Returns the instance</returns>
        public static T GetInstance<T>()
        {
            return (T)GetInstance(typeof(T));
        }

        /// <summary>
        ///     Check if Instance is already registered which can be created or is already created and returns the instance.
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ContainerException"></exception>
        /// <returns>The Instance with the given type</returns>
        private static object GetInstance(Type type)
        {
            lock (DependencyLock)
            {
                if (Storage.TryGetValue(type, out var fac)) return fac();
            }

            if (!type.IsAbstract) return CreateInstance(type);
            throw new ContainerException("No registration for " + type);
        }

        /// <summary>
        ///     Creates a new Instance every time GetInstance get invoked, excepts Singleton
        /// </summary>
        /// <param name="implementationType"></param>
        /// <returns></returns>
        private static object CreateInstance(Type implementationType)
        {
            // Only supports one ctor
            var ctor = implementationType.GetConstructors().Single();
            var paramTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = paramTypes.Select(GetInstance).ToArray();
            return Activator.CreateInstance(implementationType, dependencies);
        }

        private static void Add(Type type, Func<object> func)
        {
            lock (DependencyLock)
            {
                if (!Storage.ContainsKey(type)) Storage.Add(type, func);
            }
        }
    }
}