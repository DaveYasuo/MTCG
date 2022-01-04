using System;
using System.Collections.Generic;
using ServerModule.SimpleLogic.Handler;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Mapping
{
    public class Mapping : IMapping
    {
        public Dictionary<Method, Dictionary<string, Func<RequestHandlerData, object>>> MappingPath { get; }

        public Mapping()
        {
            MappingPath = new Dictionary<Method, Dictionary<string, Func<RequestHandlerData, object>>>();
            Initialize();
        }

        public void Initialize()
        {
            // Convert Enum to List
            // See: https://stackoverflow.com/a/1167367
            List<Method> methods = new List<Method>(Enum.GetValues<Method>());

            // Delete unsupported methods
            methods.Remove(Method.Patch);
            methods.Remove(Method.Error);

            foreach (Method method in methods)
            {
                MappingPath.Add(method, RequestHandler.GetMethodHandler(method));
            }
        }

        public bool Contains(Method method, string path)
        {
            return MappingPath[method].ContainsKey(path);
        }

        public Response InvokeMethod(Method method, string path, string token, object payload, string pathVariable, string requestParam)
        {
            return (Response)MappingPath[method][path].Invoke(new RequestHandlerData(token, payload, pathVariable, requestParam));
        }
    }
}