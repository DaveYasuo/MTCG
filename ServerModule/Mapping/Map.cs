using System;
using System.Collections.Generic;
using ServerModule.Requests;
using ServerModule.Responses;
using ServerModule.Security;
using ServerModule.Utility;

namespace ServerModule.Mapping
{
    public class Map : IMap
    {
        // Passing function to mapping
        // See: https://stackoverflow.com/a/2082650
        private readonly Dictionary<Method, Dictionary<string, Func<RequestData, Response>>> _mappingPath;

        public Map(IRequestHandler requestHandler)
        {
            _mappingPath = new Dictionary<Method, Dictionary<string, Func<RequestData, Response>>>();
            // Convert Enum to List
            // See: https://stackoverflow.com/a/1167367
            var methods = new List<Method>(Enum.GetValues<Method>());
            foreach (var method in methods) _mappingPath.Add(method, requestHandler.GetMethodHandler(method));
        }

        public bool Contains(Method method, string path)
        {
            return _mappingPath[method].ContainsKey(path);
        }

        public Response InvokeMethod(Method method, string path, AuthToken token, string payload, string pathVariable,
            string requestParam)
        {
            return _mappingPath[method][path].Invoke(new RequestData(token, payload, pathVariable, requestParam));
        }
    }
}