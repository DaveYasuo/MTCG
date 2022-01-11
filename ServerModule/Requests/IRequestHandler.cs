using System;
using System.Collections.Generic;
using ServerModule.Mapping;
using ServerModule.Responses;
using ServerModule.Utility;

namespace ServerModule.Requests
{
    public interface IRequestHandler
    {
        public Dictionary<string, Func<RequestData, Response>> GetMethodHandler(Method method);
    }
}