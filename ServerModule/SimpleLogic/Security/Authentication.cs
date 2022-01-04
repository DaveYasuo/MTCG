using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    /// <summary>
    /// Wrapper Class for Security
    /// </summary>
    public class Authentication
    {
        private static readonly ISecurity Security = new Security();
        
        /// <summary>
        /// Checks if the Request needs Authentication
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>Returns true if secured, otherwise false</returns>
        public static bool PathIsSecured(Method method, string path)
        {
            return Security.SecuredPaths()[method].Contains(path);
        }

        public static bool Check(string type, string token)
        {
            return Security.Authenticate(type, token);
        }
    }
}