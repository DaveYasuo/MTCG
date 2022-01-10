using ServerModule.Database.Models;
using ServerModule.Utility;

namespace ServerModule.SimpleLogic.Security
{
    /// <summary>
    /// Wrapper Class for Security
    /// </summary>
    public static class Authentication
    {
        private static readonly ISecurity Security = new Security();

        /// <summary>
        /// Checks if the Request needs Authentication
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>Returns true if secured, otherwise false</returns>
        public static bool PathIsSecured(this Method method, string path)
        {
            return Security.SecuredPaths()[method].Contains(path);
        }

        /// <summary>
        /// Checks if User is logged in
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool Check(this string type, string token)
        {
            return Security.Authenticate(type, token);
        }

        /// <summary>
        /// Registering User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True on success, else false.</returns>
        public static bool Register(this User user)
        {
            return Security.Register(user);
        }

        /// <summary>
        /// Check if user is registered.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the generated authentication token or else null</returns>
        public static string Login(this User user)
        {
            return Security.Login(user);
        }

        public static AuthToken GetDetails(this string token)
        {
            return Security.GetTokenDetails(token);
        }

        public static bool UpdateGameStatus(this string token, bool setStatus)
        {
            return Security.UpdateGameStatus(token, setStatus);
        }
    }
}