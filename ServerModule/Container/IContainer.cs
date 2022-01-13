using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServerModule.Exceptions;

namespace ServerModule.Container
{
    public interface IContainer
    {
        // Generic methods for adding instances to the container
        // See: https://stackoverflow.com/a/15717047
        /// <summary>
        /// TService is the Interface, TImpl is the Class which implements the TService
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImpl"></typeparam>
        public void Register<TService, TImpl>() where TImpl : TService;

        /// <summary>
        /// Use this method when using the factory design pattern
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        public void Register<TService>(Func<TService> factory);

        /// <summary>
        /// Use this method like "new Class(paramsCanBeEmpty)"
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instance"></param>
        public void RegisterInstance<TService>(TService instance);

        /// <summary>
        /// Use this method when using the factory design pattern and only one Instance is wanted
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="factory"></param>
        public void RegisterSingleton<TService>(Func<TService> factory);

        /// <summary>
        /// Use this method, when only one instance is needed, but it depends on other Classes
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public void RegisterSingleton<TService>();

        /// <summary>
        /// Gets the specific Instance, which can be Singleton or a new Instance. It depends on how it was registered.
        /// </summary>
        /// <returns>Returns the instance</returns>
        public T GetInstance<T>();
    }
}