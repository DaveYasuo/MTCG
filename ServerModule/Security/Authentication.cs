using ServerModule.Utility;

namespace ServerModule.Security
{
    /// <summary>
    /// Wrapper Class for Security
    /// </summary>
    public class Authentication
    {
        private readonly ISecurity _security;

        public Authentication(ISecurity security)
        {
            _security = security;
        }

        /// <summary>
        /// Checks if the Request needs Authentication
        /// </summary>
        /// <param name="method"></param>
        /// <param name="path"></param>
        /// <returns>Returns true if secured, otherwise false</returns>
        public bool PathIsSecured(Method method, string path)
        {
            return _security.SecuredPaths()[method].Contains(path);
        }

        /// <summary>
        /// Checks if User is logged in
        /// </summary>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns>True if already logged in with the specific statusCode, else false</returns>
        public bool Check(string type, string token, UserStatus statusCode)
        {
            return _security.Authenticate(type, token, statusCode);
        }

        /// <summary>
        /// Registering User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>True on success, else false.</returns>
        public bool Register(User user)
        {
            return _security.Register(user);
        }

        /// <summary>
        /// Check if user is registered.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Returns the generated authentication token or else null</returns>
        public string Login(User user)
        {
            return _security.Login(user);
        }

        public AuthToken GetDetails(string token)
        {
            return _security.GetTokenDetails(token);
        }

        public bool UpdateStatus(string token, UserStatus newStatus, UserStatus oldStatus)
        {
            return _security.UpdateStatus(token, newStatus, oldStatus);
        }
    }
}