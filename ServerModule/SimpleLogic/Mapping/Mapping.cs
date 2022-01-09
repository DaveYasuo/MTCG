using System;
using System.Collections.Generic;
using ServerModule.SimpleLogic.Handler;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;
using RequestHandler = ServerModule.SimpleLogic.Handler.RequestHandling.RequestHandler;

namespace ServerModule.SimpleLogic.Mapping
{
    public class Mapping : IMapping
    {
        public Dictionary<Method, Dictionary<string, Func<RequestData, Response>>> MappingPath { get; }

        public Mapping()
        {
            MappingPath = new Dictionary<Method, Dictionary<string, Func<RequestData, Response>>>();
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

        public Response InvokeMethod(Method method, string path, string username, string payload, string pathVariable, string requestParam)
        {
            return MappingPath[method][path].Invoke(new RequestData(username, payload, pathVariable, requestParam));
        }
    }
}