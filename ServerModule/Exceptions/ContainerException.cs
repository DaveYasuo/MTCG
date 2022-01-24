using System;

namespace ServerModule.Exceptions
{
    /// <summary>
    ///     Use this Exception when using IoC-Container
    /// </summary>
    public class ContainerException : Exception
    {
        // create custom exception
        // See: https://docs.microsoft.com/en-us/dotnet/standard/exceptions/how-to-create-user-defined-exceptions
        /// <summary>
        ///     Use this Exception when using IoC-Container
        /// </summary>
        public ContainerException()
        {
        }

        /// <summary>
        ///     Use this Exception when using IoC-Container
        /// </summary>
        /// <param name="message"></param>
        public ContainerException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Use this Exception when using IoC-Container
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public ContainerException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}