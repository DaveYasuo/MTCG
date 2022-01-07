using System;
using System.Collections.Generic;
using ServerModule.SimpleLogic.Handler;
using ServerModule.SimpleLogic.Responses;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Mapping
{
    public interface IMapping
    {
        Dictionary<Method, Dictionary<string, Func<RequestHandlerData, Response>>> MappingPath { get; }

        /// <summary>
        /// Checks if routing path exists.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>True if an endpoint is mapped, else false.</returns>
        bool Contains(Method method, string path);

        /// <summary>
        /// Invokes the method for the corresponding method and path.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="username"></param>
        /// <param name="payload"></param>
        /// <param name="pathVariable"></param>
        /// <param name="requestParam"></param>
        /// <returns>Returns a Response object</returns>
        Response InvokeMethod(Method method, string path, string username, string payload, string pathVariable, string requestParam);
    }
}