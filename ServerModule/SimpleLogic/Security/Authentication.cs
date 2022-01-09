using ServerModule.Database.Models;
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

        /// <summary>
        /// Checks if User is logged in
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Check(string type, string token)
        {
            return Security.Authenticate(type, token);
        }

        /// <summary>
        /// Registering User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True on success, else false.</returns>
        public static bool Register(User user)
        {
            return Security.Register(user);
        }

        public static string Login(User user)
        {
            return Security.Login(user);
        }

        public static AuthToken GetTokenDetails(string token)
        {
            return Security.GetTokenDetails(token);
        }
    }
}