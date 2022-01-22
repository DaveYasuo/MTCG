using System;
using System.Collections.Generic;
using ServerModule.Mapping;
using ServerModule.Responses;
using ServerModule.Utility;

namespace ServerModule.Requests
{
    public interface IRequestHandler
    {
        /// <summary>
        /// Mapping path as string to the corresponding function all stored in a dictionary.
        /// </summary>
        /// <param name="method"></param>
        /// <returns>Depends on which method is requested, the map as a dictionary will be returned.</returns>
        public Dictionary<string, Func<RequestData, Response>> GetMethodHandler(Method method);
    }
}