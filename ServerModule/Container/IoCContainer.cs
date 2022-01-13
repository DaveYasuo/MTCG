using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ServerModule.Exceptions;

namespace ServerModule.Container
{
    // watching tutorials
    // See: https://www.youtube.com/watch?v=VhASKWm_B9M
    // not thread safe
    public class IoCContainer
    {
        //private readonly IDictionary<Type, Tuple<Type, Lifetime>> _storage = new Dictionary<Type, Tuple<Type, Lifetime>>();
        //private readonly IDictionary<Type, object> _singletonContainer = new Dictionary<Type, object>();

        //public void Register<TInterface, TImplementation>(Lifetime lifetime = Lifetime.Instance)
        //    where TImplementation : TInterface, new()
        //{
        //    Type interfaceType = typeof(TInterface);
        //    Type implementationType = typeof(TImplementation);
        //    if (_storage.ContainsKey(interfaceType))
        //    {
        //        _storage[interfaceType] = new Tuple<Type, Lifetime>(implementationType, lifetime);
        //    }
        //    else
        //    {
        //        _storage.Add(new KeyValuePair<Type, Tuple<Type, Lifetime>>(interfaceType, new Tuple<Type, Lifetime>(implementationType, lifetime)));
        //    }
        //}

        // https://stackoverflow.com/a/57408320
        //private object Resolve(Type type)
        //{
        //    Type resolvedType;
        //    try
        //    {
        //        resolvedType = _storage[type];
        //        Trace.TraceInformation("Resolving {0}", type.Name);
        //    }
        //    catch (Exception resolveException)
        //    {
        //        Trace.TraceError("Couldn't resolve type {0}", resolveException);
        //        throw new IoCException("Couldn't resolve type", resolveException);
        //    }
        //    ConstructorInfo ctor = resolvedType.GetConstructors().First();
        //    ParameterInfo[] ctorParameters = ctor.GetParameters();
        //    if (ctorParameters.Length == 0)
        //    {
        //        Trace.TraceInformation("Constructor have no parameters");
        //        return Activator.CreateInstance(resolvedType);
        //    }

        //    var parameters = new System.Collections.Generic.List<object>();
        //    Trace.TraceInformation("Constructor found to have {0} parameters", ctorParameters.Length);

        //    foreach (var p in ctorParameters)
        //    {
        //        parameters.Add(Resolve(p.ParameterType));
        //    }
        //    return ctor.Invoke(parameters.ToArray());
        //}

        //public TInterface Resolve<TInterface>()
        //{
        //    Type interfaceType = typeof(TInterface);
        //    if (!_storage.ContainsKey(interfaceType))
        //    {
        //        throw new InvalidOperationException($"{interfaceType} is not registered yet. Please do so first!");
        //    }

        //    Tuple<Type, Lifetime> implementTypeAndLifeTime = _storage[interfaceType];
        //    if (implementTypeAndLifeTime.Item2 != Lifetime.Singleton) return (TInterface)Activator.CreateInstance(implementTypeAndLifeTime.Item1);

        //    // Check if singleton already exist if so return it, otherwise create it
        //    if (!_singletonContainer.ContainsKey(implementTypeAndLifeTime.Item1))
        //    {
        //        _singletonContainer.Add(new KeyValuePair<Type, object>(implementTypeAndLifeTime.Item1, Activator.CreateInstance(implementTypeAndLifeTime.Item1)));
        //    }
        //    return (TInterface)_singletonContainer[implementTypeAndLifeTime.Item1];


        //    return (TInterface)Resolve(typeof(TInterface));
        //}
        // See: https://stackoverflow.com/a/15717047
        private readonly Dictionary<Type, Func<object>> _storage = new();

        public void Register<TService, TImpl>() where TImpl : TService =>
            _storage.Add(typeof(TService), () => GetInstance(typeof(TImpl)));

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
            TService instance = (TService)GetInstance(typeof(TService));
            TService Factory() => instance;
            // lazy for thread safety
            // See: https://riptutorial.com/csharp/example/6795/
            Lazy<TService> lazy = new Lazy<TService>(Factory);
            Register(() => lazy.Value);
        }

        public object GetInstance(Type type)
        {
            if (_storage.TryGetValue(type, out Func<object> fac)) return fac();
            if (!type.IsAbstract) return CreateInstance(type);
            throw new ContainerException("No registration for " + type);
        }

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