using ServerModule.Responses;
using ServerModule.Security;
using ServerModule.Utility;

namespace ServerModule.Mapping
{
    public interface IMap
    {
        /// <summary>
        ///     Checks if routing path exists.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>True if an endpoint is mapped, else false.</returns>
        bool Contains(Method method, string path);

        /// <summary>
        ///     Invokes the method for the corresponding method and path.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="payload"></param>
        /// <param name="pathVariable"></param>
        /// <param name="requestParam"></param>
        /// <returns>Returns a Response object</returns>
        Response InvokeMethod(Method method, string path, AuthToken token, string payload, string pathVariable,
            string requestParam);
    }
}